import type { Category, Product } from '../types';
import type { ApiCategory, ApiProduct, ApiDistrict } from './types';
import type { District } from '../types';
import { calculateFreshness } from '../utils/freshness';

const PLACEHOLDER_IMAGE =
  'https://images.pexels.com/photos/931796/pexels-photo-931796.jpeg?auto=compress&cs=tinysrgb&w=600';

export function slugify(text: string): string {
  return text
    .toLowerCase()
    .replace(/ğ/g, 'g')
    .replace(/ü/g, 'u')
    .replace(/ş/g, 's')
    .replace(/ı/g, 'i')
    .replace(/ö/g, 'o')
    .replace(/ç/g, 'c')
    .replace(/[^a-z0-9]+/g, '-')
    .replace(/(^-|-$)/g, '');
}

function mapBadge(freshnessPercentage: number, stock: number): Product['badge'] | undefined {
  if (freshnessPercentage >= 90) return 'Yeni';
  if (stock >= 20) return 'Çok Satan';
  return undefined;
}

export function mapProduct(api: ApiProduct): Product {
  const slug = `${slugify(api.name)}-${api.id}`;
  const categoryName = api.category?.name ?? '';
  const freshness = calculateFreshness({
    freshnessScore: api.freshnessScore,
    stock: api.stock,
    defaultVaseLifeDays: api.defaultVaseLifeDays,
  });

  return {
    id: String(api.id),
    name: api.name,
    slug,
    categoryId: String(api.categoryId),
    price: api.price,
    images: api.imageUrl ? [api.imageUrl] : [PLACEHOLDER_IMAGE],
    description: api.category?.description || `${api.name} — taze ve özenle hazırlanmış.`,
    longDescription: `${api.name} ${freshness.label.toLowerCase()} durumda hazırlanır. Vazoda yaklaşık ${api.defaultVaseLifeDays} gün taze kalır.${categoryName ? ` ${categoryName} kategorisinde yer alır.` : ''}`,
    ingredients: categoryName ? [categoryName, 'Taze çiçekler', 'Dekoratif yeşillik'] : ['Taze çiçekler', 'Dekoratif yeşillik'],
    freshness,
    badge: mapBadge(freshness.percentage, api.stock),
    inStock: api.stock > 0,
    deliveryInfo: `Aynı gün teslimat · ${api.defaultVaseLifeDays} gün taze`,
  };
}

export function mapCategory(api: ApiCategory, products: ApiProduct[]): Category {
  const categoryProducts = products.filter((p) => p.categoryId === api.id);
  const image =
    categoryProducts[0]?.imageUrl ||
    api.products?.[0]?.imageUrl ||
    PLACEHOLDER_IMAGE;

  return {
    id: String(api.id),
    name: api.name,
    slug: slugify(api.name),
    description: api.description,
    image,
    icon: 'Flower2',
  };
}

export function mapDistrict(api: ApiDistrict): District {
  return {
    id: api.id,
    name: api.name,
    baseDeliveryFee: api.baseDeliveryFee,
  };
}
