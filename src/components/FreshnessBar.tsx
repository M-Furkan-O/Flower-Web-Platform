import type { Freshness } from '../types';
import { freshnessBarColor } from '../utils/freshness';

type Props = {
  freshness: Freshness;
  size?: 'sm' | 'md';
  showVaseLife?: boolean;
};

export default function FreshnessBar({ freshness, size = 'sm', showVaseLife = false }: Props) {
  const barHeight = size === 'md' ? 'h-2.5' : 'h-1.5';
  const textSize = size === 'md' ? 'text-sm' : 'text-xs';

  if (freshness.percentage === 0) {
    return (
      <div className={textSize}>
        <span className="font-medium text-red-600">{freshness.label}</span>
      </div>
    );
  }

  return (
    <div className="space-y-1">
      <div className="flex items-center justify-between gap-2">
        <span className={`${textSize} font-medium text-sand-700`}>{freshness.label}</span>
        <span className={`${textSize} text-sand-400 tabular-nums`}>%{freshness.percentage}</span>
      </div>
      <div className={`w-full ${barHeight} bg-sand-100 rounded-full overflow-hidden`}>
        <div
          className={`${barHeight} rounded-full transition-all duration-500 ${freshnessBarColor(freshness.percentage)}`}
          style={{ width: `${freshness.percentage}%` }}
        />
      </div>
      {showVaseLife && (
        <p className={`${textSize} text-sand-400`}>Vazoda ~{freshness.vaseLifeDays} gün taze kalır</p>
      )}
    </div>
  );
}
