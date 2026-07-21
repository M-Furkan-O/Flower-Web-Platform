# Flower-Web-Platform
An enterprise-level online floral delivery platform built with .NET 8, React, and PostGIS. Features a dynamic freshness engine, spatial delivery calculations, and a Zettelkasten-based botanical knowledge network.


# 🌸 Çevrimiçi Çiçekçi Dükkanı ve Botanik Wiki Platformu
**Proje Dokümantasyonu & Sistem Mimarisi**

* **Ders:** Work Experience (WEX328) - GitHub & Vibe Coding  
* **Ekip Üyeleri:** Şehriban Ümmü İnce, Hilal, Furkan  
* **Hedef:** Canlı ortama alınabilir, gerçek bir işletmeye sunulup satılabilecek kalitede E-Ticaret Platformu  
* **Referans Arayüz:** [Meritflower Ankara Online Çiçek](https://www.meritflower.com/ankara-online-cicek)  

---

## 1. Projenin Amacı ve Kapsamı

### 🎯 Projenin Amacı
Bu projenin temel amacı; geleneksel e-ticaret sistemlerinin ötesine geçerek, **gerçek bir çiçekçilik işletmesinin uçtan uca dijitalleşmesini sağlayacak**, canlı ortama alınabilir, modüler, güvenli ve yüksek ölçeklenebilir bir platform geliştirmektir.

### 🔍 Projenin Kapsamı
Uygulama iki ana kullanıcı rolü üzerinden kurgulanmıştır:

1. **Müşteri (Alıcı) Deneyimi:**
   * Referans sitedeki gibi sade, göz yormayan, ürün odaklı ve modern kart yapısıyla ürün listeleme.
   * **PostGIS ile Dinamik Bölge/Teslimat Ücreti:** Müşterinin seçtiği ilçeye (Çankaya, Keçiören, Polatlı vb.) ve mağaza uzaklığına göre teslimat ücretinin otomatik hesaplanması.
   * **Tazelik ve Canlılık Motoru (Freshness Engine):** Çiçeklerin anlık tazelik durumunu ve tahmini vazo ömrünü gösteren dinamik ilerleme çubukları (Progress Bar).
   * **Botanikal Wiki (Zettelkasten):** Çiçek bakımı, türleri ve aranjman detayları hakkında atomik notlar sunan interaktif rehber.
   * **İnteraktif Ödeme Geribildirimi (Gamification):** Müşteri ödemeyi tamamladığı anda, kullanıcı deneyimini (UX) eğlenceli ve akılda kalıcı kılmak adına ekranda çiçeğin sulanarak canlandığını gösteren mikro bir kutlama animasyonu (Lottie / CSS Animation).
   * **Sepet ve Sipariş Takibi:** Dinamik sepet yönetimi ve canlı sipariş durum takibi.

2. **Dükkan Sahibi (Satıcı/Admin) Deneyimi:**
   * Ürün, stok ve fiyat yönetimi (CRUD).
   * İlçe bazlı teslimat ücreti ve katsayı düzenleme paneli.
   * Canlı sipariş takip paneli (Hazırlanıyor ➔ Kuryede ➔ Teslim Edildi).

---

## 2. Kullanılan Teknolojiler ve Tercih Gerekçeleri

Hocamızın belirttiği "canlıya alınabilir ve satılabilir kalite" vizyonuna uygun olarak seçilen teknoloji yığını ve tercih gerekçeleri şunlardır:

### A. Frontend Teknolojileri
* **React.js & Tailwind CSS:**
  * *Tercih Gerekçesi:* Esnek bileşen (component) yapısı, yüksek render performansı ve Tailwind'in sağladığı hızlı, modern ve responsive UI prototipleme imkanı.
* **Lucide React (`lucide-react`):**
  * *Tercih Gerekçesi:* Çiçekçi estetiğine uygun; minimalist, vektörel ve sayfa yüklenme hızını olumsuz etkilemeyen modern ikon seti ihtiyacı için.

### B. Backend Teknolojileri
* **.NET 8 (ASP.NET Core Web API):**
  * *Tercih Gerekçesi:* Kurumsal (Enterprise) düzeyde yüksek güvenlik, tip güvenliği (Type-safety), LINQ ile güçlü veri sorgulama ve yüksek REST API performansı.
* **Clean Architecture & CQRS (MediatR):**
  * *Tercih Gerekçesi:* İş mantığı (Business Logic) ile veri katmanını birbirinden ayırarak projenin sürdürülebilirliğini sağlamak ve ileride yeni modüllerin kolayca entegre edilmesine imkan tanımak için.

### C. Veritabanı ve Konteyner Teknolojileri
* **PostgreSQL + PostGIS Eklentisi:**
  * *Tercih Gerekçesi:* İlişkisel veritabanı performansının yanı sıra, **PostGIS** eklentisi sayesinde harita ve konum tabanlı mekansal (spatial) uzaklık hesaplamalarını (`ST_Distance`) veritabanı seviyesinde hızlıca yapabilmek için.
* **Redis Cache:**
  * *Tercih Gerekçesi:* Kullanıcı sepetlerini ve sık erişilen Zettelkasten botanik notlarını bellek içi (in-memory) önbelleğe alarak sistem yanıt süresini minimuma indirmek için.
* **Docker & Docker Compose:**
  * *Tercih Gerekçesi:* `.NET API`, `PostgreSQL/PostGIS` ve `React` servislerini izole konteynerlar halinde paketleyerek, tüm ekibin ortam bağımsız tek komutla (`docker-compose up`) projeyi ayağa kaldırabilmesi için.
* **Iyzico Sandbox (Opsiyonel / Bonus Modül):**
  * *Tercih Gerekçesi:* Proje takvimine bağlı olarak, güvenli kartla ödeme akışını test etmek üzere Iyzico Sandbox altyapısının entegre edilmesi hedeflenmektedir.

---

## 3. Sistem Mimarisi & Veri Akışı (Mermaid.js)

```mermaid
graph TD
    Client[Web İstemci - React.js + Lucide Icons] -->|HTTP / REST| API[ASP.NET Core API Gateway]
    
    API --> Auth[Identity & JWT Auth]
    API --> Catalog[Ürün & Stok Servisi]
    API --> PostGIS_Service[PostGIS İlçe/Mesafe Servisi]
    API --> Order[Sipariş & Sepet Servisi]
    API --> Zettel[Zettelkasten Botanik Wiki]

    Auth --> DB[(PostgreSQL + PostGIS)]
    Catalog --> DB
    PostGIS_Service --> DB
    Order --> DB
    Zettel --> DB

    Catalog --> Cache[(Redis Cache)]
    Order --> Cache
    
    Order --> Payment[Iyzico Sandbox - Opsiyonel]
