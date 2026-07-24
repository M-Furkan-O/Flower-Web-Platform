import { apiFetch } from './client';
import type { ApiCategory, ApiProduct, ApiDistrict } from './types';
import { mapCategory, mapProduct, mapDistrict } from './mappers';
import type { Category, Product, District } from '../types';

export async function fetchCategories(): Promise<Category[]> {
  const data = await apiFetch<ApiCategory[]>('/categories');
  const products = await apiFetch<ApiProduct[]>('/products');
  return data.map((c) => mapCategory(c, products));
}

export async function fetchProducts(): Promise<Product[]> {
  const data = await apiFetch<ApiProduct[]>('/products');
  return data.map(mapProduct);
}

export async function fetchDistricts(): Promise<District[]> {
  const data = await apiFetch<ApiDistrict[]>('/districts');
  return data.map(mapDistrict);
}

export async function fetchCatalog(): Promise<{
  categories: Category[];
  products: Product[];
  districts: District[];
}> {
  const [apiCategories, apiProducts, districts] = await Promise.all([
    apiFetch<ApiCategory[]>('/categories'),
    apiFetch<ApiProduct[]>('/products'),
    apiFetch<ApiDistrict[]>('/districts'),
  ]);

  const products = apiProducts.map(mapProduct);
  const categories = apiCategories.map((c) => mapCategory(c, apiProducts));

  return { categories, products, districts: districts.map(mapDistrict) };
}
