import type { Freshness } from '../types';

type FreshnessInput = {
  freshnessScore: number;
  stock: number;
  defaultVaseLifeDays: number;
};

const LABELS: { min: number; label: string }[] = [
  { min: 90, label: 'Çok Taze' },
  { min: 75, label: 'Taze' },
  { min: 60, label: 'İyi' },
  { min: 40, label: 'Orta' },
  { min: 0, label: 'Sınırlı' },
];

function freshnessLabel(percentage: number): string {
  return LABELS.find((l) => percentage >= l.min)?.label ?? 'Sınırlı';
}

/**
 * Tazelik endeksi: florist kalite puanı, stok durumu ve vazo ömrünü birleştirir.
 * - Kalite (%55): veritabanındaki freshnessScore (1–10)
 * - Stok (%25): stok yoksa 0; az stok hafif düşüş; yeterli stok tam puan
 * - Vazo ömrü (%20): defaultVaseLifeDays / 10 gün referans
 */
export function calculateFreshness(input: FreshnessInput): Freshness {
  if (input.stock <= 0) {
    return { percentage: 0, label: 'Tükendi', vaseLifeDays: input.defaultVaseLifeDays };
  }

  const quality = (Math.min(10, Math.max(1, input.freshnessScore)) / 10) * 55;

  const stock =
    input.stock <= 2 ? 12 :
    input.stock <= 5 ? 18 :
    input.stock <= 10 ? 22 :
    25;

  const vase = Math.min(20, (input.defaultVaseLifeDays / 10) * 20);

  const percentage = Math.round(Math.min(100, quality + stock + vase));

  return {
    percentage,
    label: freshnessLabel(percentage),
    vaseLifeDays: input.defaultVaseLifeDays,
  };
}

export function freshnessBarColor(percentage: number): string {
  if (percentage >= 90) return 'bg-leaf-500';
  if (percentage >= 75) return 'bg-leaf-400';
  if (percentage >= 60) return 'bg-amber-400';
  if (percentage >= 40) return 'bg-orange-400';
  return 'bg-red-400';
}
