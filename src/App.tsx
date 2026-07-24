import { useState, useCallback, useMemo } from 'react';
import { useRouter } from './router';
import { useCart } from './useCart';
import { useCatalog } from './hooks/useCatalog';
import { createOrder } from './api/orders';
import type { Product, OrderInfo } from './types';
import Header from './components/Header';
import Footer from './components/Footer';
import Toast from './components/Toast';
import HomePage from './pages/HomePage';
import ShopPage from './pages/ShopPage';
import ProductPage from './pages/ProductPage';
import CartPage from './pages/CartPage';
import CheckoutPage from './pages/CheckoutPage';
import OrderSuccessPage from './pages/OrderSuccessPage';
import AboutPage from './pages/AboutPage';
import ContactPage from './pages/ContactPage';
import FaqPage from './pages/FaqPage';

function App() {
  const { route, navigate } = useRouter();
  const cart = useCart();
  const catalog = useCatalog();
  const [toast, setToast] = useState<string | null>(null);
  const [orders, setOrders] = useState<Record<string, OrderInfo>>({});
  const [selectedDistrictId, setSelectedDistrictId] = useState<number | null>(null);

  const selectedDistrict = useMemo(
    () => catalog.districts.find((d) => d.id === selectedDistrictId) ?? null,
    [catalog.districts, selectedDistrictId],
  );

  const deliveryFee = selectedDistrict?.baseDeliveryFee ?? 0;
  const total = cart.subtotal + deliveryFee;

  const showToast = useCallback((msg: string) => {
    setToast(null);
    setTimeout(() => setToast(msg), 50);
  }, []);

  const handleAddToCart = useCallback(
    (product: Product, qty: number = 1) => {
      cart.addItem(product, qty);
      showToast(`${product.name} sepete eklendi`);
    },
    [cart, showToast],
  );

  const handlePlaceOrder = useCallback(
    async (
      orderData: Omit<OrderInfo, 'id' | 'createdAt' | 'status'> & { districtId: number },
    ): Promise<string> => {
      const response = await createOrder({
        customerName: orderData.recipientName,
        customerPhone: orderData.recipientPhone,
        address: orderData.address,
        districtId: orderData.districtId,
        items: orderData.items.map((i) => ({
          productId: Number(i.product.id),
          quantity: i.quantity,
        })),
      });

      const order: OrderInfo = {
        items: orderData.items,
        total: response.grandTotal,
        recipientName: orderData.recipientName,
        recipientPhone: orderData.recipientPhone,
        address: orderData.address,
        city: response.districtName,
        deliveryDate: orderData.deliveryDate,
        note: orderData.note,
        id: String(response.orderId),
        createdAt: response.orderDate,
        status: 'Hazırlanıyor',
      };

      setOrders((prev) => ({ ...prev, [String(response.orderId)]: order }));
      cart.clearCart();
      setSelectedDistrictId(null);
      return String(response.orderId);
    },
    [cart],
  );

  if (catalog.loading) {
    return (
      <div className="min-h-screen flex items-center justify-center bg-sand-50">
        <div className="text-center">
          <div className="w-10 h-10 border-3 border-brand-200 border-t-brand-600 rounded-full animate-spin mx-auto" />
          <p className="text-sand-500 mt-4">Yükleniyor...</p>
        </div>
      </div>
    );
  }

  if (catalog.error) {
    return (
      <div className="min-h-screen flex items-center justify-center bg-sand-50 px-4">
        <div className="text-center max-w-md">
          <h1 className="font-display text-2xl font-bold text-sand-900">Bağlantı hatası</h1>
          <p className="text-sand-500 mt-2">{catalog.error}</p>
          <p className="text-sm text-sand-400 mt-2">API sunucusunun çalıştığından emin olun.</p>
          <button onClick={catalog.reload} className="btn-primary mt-6">
            Tekrar Dene
          </button>
        </div>
      </div>
    );
  }

  const renderPage = () => {
    switch (route.name) {
      case 'home':
        return (
          <HomePage
            categories={catalog.categories}
            featured={catalog.featured}
            discounted={catalog.discounted}
            navigate={navigate}
            onAddToCart={handleAddToCart}
          />
        );
      case 'shop':
        return (
          <ShopPage
            products={catalog.products}
            categories={catalog.categories}
            activeCategorySlug={route.categorySlug}
            navigate={navigate}
            onAddToCart={handleAddToCart}
          />
        );
      case 'product': {
        const product = catalog.getProductBySlug(route.slug);
        if (!product) {
          return (
            <div className="max-w-2xl mx-auto px-4 py-20 text-center">
              <h1 className="font-display text-2xl font-bold text-sand-900">Ürün bulunamadı</h1>
              <button onClick={() => navigate({ name: 'shop' })} className="btn-primary mt-6">Mağazaya Dön</button>
            </div>
          );
        }
        return (
          <ProductPage
            product={product}
            categories={catalog.categories}
            products={catalog.products}
            navigate={navigate}
            onAddToCart={handleAddToCart}
          />
        );
      }
      case 'cart':
        return (
          <CartPage
            items={cart.items}
            subtotal={cart.subtotal}
            deliveryFee={deliveryFee}
            total={total}
            navigate={navigate}
            onUpdateQuantity={cart.updateQuantity}
            onRemove={cart.removeItem}
          />
        );
      case 'checkout':
        return (
          <CheckoutPage
            items={cart.items}
            subtotal={cart.subtotal}
            deliveryFee={deliveryFee}
            total={total}
            districts={catalog.districts}
            selectedDistrictId={selectedDistrictId}
            onDistrictChange={setSelectedDistrictId}
            navigate={navigate}
            onPlaceOrder={handlePlaceOrder}
          />
        );
      case 'order-success': {
        const order = orders[route.orderId];
        return <OrderSuccessPage order={order} navigate={navigate} />;
      }
      case 'about':
        return <AboutPage navigate={navigate} />;
      case 'contact':
        return <ContactPage navigate={navigate} />;
      case 'faq':
        return <FaqPage navigate={navigate} />;
      default:
        return (
          <HomePage
            categories={catalog.categories}
            featured={catalog.featured}
            discounted={catalog.discounted}
            navigate={navigate}
            onAddToCart={handleAddToCart}
          />
        );
    }
  };

  return (
    <div className="min-h-screen flex flex-col bg-sand-50">
      <Header cartCount={cart.totalItems} navigate={navigate} currentRoute={route} />
      <main className="flex-1">{renderPage()}</main>
      <Footer navigate={navigate} />
      {toast && <Toast message={toast} onClose={() => setToast(null)} />}
    </div>
  );
}

export default App;
