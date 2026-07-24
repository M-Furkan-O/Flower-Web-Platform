import { useState, useEffect, useCallback, useMemo } from 'react';
import { fetchCatalog } from '../api/catalog';
import type { Category, Product, District } from '../types';

export function useCatalog() {
  const [categories, setCategories] = useState<Category[]>([]);
  const [products, setProducts] = useState<Product[]>([]);
  const [districts, setDistricts] = useState<District[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const load = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      const data = await fetchCatalog();
      setCategories(data.categories);
      setProducts(data.products);
      setDistricts(data.districts);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Veriler yüklenemedi');
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    load();
  }, [load]);

  const getProductBySlug = useCallback(
    (slug: string) => products.find((p) => p.slug === slug),
    [products],
  );

  const getCategoryById = useCallback(
    (id: string) => categories.find((c) => c.id === id),
    [categories],
  );

  const featured = useMemo(
    () => products.filter((p) => p.badge).slice(0, 4),
    [products],
  );

  const discounted = useMemo(
    () => products.filter((p) => p.oldPrice !== undefined).slice(0, 4),
    [products],
  );

  return {
    categories,
    products,
    districts,
    loading,
    error,
    reload: load,
    getProductBySlug,
    getCategoryById,
    featured,
    discounted,
  };
}
