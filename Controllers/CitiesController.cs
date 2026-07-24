using FlowerShop.API.Data;
using FlowerShop.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlowerShop.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CitiesController : ControllerBase
{
    private readonly AppDbContext _context;

    public CitiesController(AppDbContext context)
    {
        _context = context;
    }

    // 1. Şehirleri Getir: GET /api/Cities
    [HttpGet]
    public async Task<ActionResult<IEnumerable<City>>> GetCities()
    {
        return await _context.Cities.OrderBy(c => c.Id).ToListAsync();
    }

    // 2. Şehre Ait İlçeleri Getir: GET /api/Cities/6/districts
    [HttpGet("{cityId}/districts")]
    public async Task<ActionResult<IEnumerable<District>>> GetDistrictsByCity(int cityId)
    {
        return await _context.Districts
            .Where(d => d.CityId == cityId)
            .OrderBy(d => d.Name)
            .ToListAsync();
    }

    // 3. İlçeye Ait Mahalleleri Getir: GET /api/Cities/districts/1/neighborhoods
    [HttpGet("districts/{districtId}/neighborhoods")]
    public async Task<ActionResult<IEnumerable<Neighborhood>>> GetNeighborhoodsByDistrict(int districtId)
    {
        return await _context.Neighborhoods
            .Where(n => n.DistrictId == districtId)
            .OrderBy(n => n.Name)
            .ToListAsync();
    }

    // 🚀 BÜTÜN TÜRKİYE'Yİ (81 İL + İLÇELERİ) KOD ÜZERİNDEN VERİTABANINA YAZAR
    [HttpGet("seed-all-turkey")]
    public async Task<IActionResult> SeedAllTurkey()
    {
        try
        {
            // 1. Eski adres verilerini temizle
            _context.Neighborhoods.RemoveRange(_context.Neighborhoods);
            _context.Districts.RemoveRange(_context.Districts);
            _context.Cities.RemoveRange(_context.Cities);
            await _context.SaveChangesAsync();

            // 2. 81 İlin Tamamı
            var rawData = GetTurkeyData();

            int districtIdCounter = 1;
            var citiesToAdd = new List<City>();
            var districtsToAdd = new List<District>();

            foreach (var item in rawData)
            {
                citiesToAdd.Add(new City { Id = item.Id, Name = item.Name });

                foreach (var distName in item.Districts)
                {
                    districtsToAdd.Add(new District
                    {
                        Id = districtIdCounter++,
                        CityId = item.Id,
                        Name = distName,
                        BaseDeliveryFee = 0
                    });
                }
            }

            // 3. Veritabanına kaydet
            await _context.Cities.AddRangeAsync(citiesToAdd);
            await _context.Districts.AddRangeAsync(districtsToAdd);
            await _context.SaveChangesAsync();

            // 4. Örnek Mahalleler (Ankara / Çankaya için)
            var cankaya = districtsToAdd.FirstOrDefault(d => d.CityId == 6 && d.Name == "Çankaya");
            if (cankaya != null)
            {
                var sampleNeighborhoods = new List<Neighborhood>
                {
                    new Neighborhood { DistrictId = cankaya.Id, Name = "Bahçelievler Mah.", Pk = "06490" },
                    new Neighborhood { DistrictId = cankaya.Id, Name = "Kızılay Mah.", Pk = "06420" },
                    new Neighborhood { DistrictId = cankaya.Id, Name = "Tunalı Hilmi Mah.", Pk = "06700" }
                };
                await _context.Neighborhoods.AddRangeAsync(sampleNeighborhoods);
                await _context.SaveChangesAsync();
            }

            return Ok(new 
            { 
                message = $"🎉 TEBRİKLER! Türkiye'nin 81 ili ve toplam {districtsToAdd.Count} ilçesi veritabanına eklendi!",
                toplamSehir = citiesToAdd.Count,
                toplamIlce = districtsToAdd.Count
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message, inner = ex.InnerException?.Message });
        }
    }

    private class CityItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string[] Districts { get; set; } = Array.Empty<string>();
    }

    // 81 İlin Tam Metin Listesi
    private List<CityItem> GetTurkeyData()
    {
        return new List<CityItem>
        {
            new CityItem { Id = 1, Name = "Adana", Districts = new[] { "Aladağ", "Ceyhan", "Çukurova", "Feke", "İmamoğlu", "Karaisalı", "Karataş", "Kozan", "Pozantı", "Saimbeyli", "Sarıçam", "Seyhan", "Tufanbeyli", "Yumurtalık", "Yüreğir" } },
            new CityItem { Id = 2, Name = "Adıyaman", Districts = new[] { "Besni", "Çelikhan", "Gerger", "Gölbaşı", "Kahta", "Merkez", "Samsat", "Sincik", "Tut" } },
            new CityItem { Id = 3, Name = "Afyonkarahisar", Districts = new[] { "Başmakçı", "Bayat", "Bolvadin", "Çay", "Çobanlar", "Dazkırı", "Dinar", "Emirdağ", "Evciler", "Hocalar", "İhsaniye", "İscehisar", "Kızılören", "Merkez", "Sandıklı", "Sinanpaşa", "Sultandağı", "Şuhut" } },
            new CityItem { Id = 4, Name = "Ağrı", Districts = new[] { "Diyadin", "Doğubayazıt", "Eleşkirt", "Hamur", "Merkez", "Patnos", "Taşlıçay", "Tutak" } },
            new CityItem { Id = 5, Name = "Amasya", Districts = new[] { "Göynücek", "Gümüşhacıköy", "Hamamözü", "Merkez", "Merzifon", "Suluova", "Taşova" } },
            new CityItem { Id = 6, Name = "Ankara", Districts = new[] { "Akyurt", "Altındağ", "Ayaş", "Bala", "Beypazarı", "Çamlıdere", "Çankaya", "Çubuk", "Elmadağ", "Etimesgut", "Evren", "Gölbaşı", "Güdül", "Haymana", "Kahramankazan", "Kalecik", "Keçiören", "Kızılcahamam", "Mamak", "Nallıhan", "Polatlı", "Pursaklar", "Sincan", "Şereflikoçhisar", "Yenimahalle" } },
            new CityItem { Id = 7, Name = "Antalya", Districts = new[] { "Akseki", "Aksu", "Alanya", "Demre", "Döşemealtı", "Elmalı", "Finike", "Gazipaşa", "Gündoğmuş", "İbradı", "Kaş", "Kemer", "Kepez", "Konyaaltı", "Korkuteli", "Kumluca", "Manavgat", "Muratpaşa", "Serik" } },
            new CityItem { Id = 8, Name = "Artvin", Districts = new[] { "Ardanuç", "Arhavi", "Borçka", "Hopa", "Kemalpaşa", "Merkez", "Murgul", "Şavşat", "Yusufeli" } },
            new CityItem { Id = 9, Name = "Aydın", Districts = new[] { "Bozdoğan", "Buharkent", "Çine", "Didim", "Efeler", "Germencik", "İncirliova", "Karacasu", "Karpuzlu", "Koçarlı", "Köşk", "Kuşadası", "Kuyucak", "Nazilli", "Söke", "Sultanhisar", "Yenipazar" } },
            new CityItem { Id = 10, Name = "Balıkesir", Districts = new[] { "Altıeylül", "Ayvalık", "Balya", "Bandırma", "Bigadiç", "Burhaniye", "Dursunbey", "Edremit", "Erdek", "Gömeç", "Gönen", "Havran", "İvrindi", "Karesi", "Kepsut", "Manyas", "Marmara", "Savaştepe", "Sındırgı", "Susurluk" } },
            new CityItem { Id = 11, Name = "Bilecik", Districts = new[] { "Bozüyük", "Gölpazarı", "İnhisar", "Merkez", "Osmaneli", "Pazaryeri", "Söğüt", "Yenipazar" } },
            new CityItem { Id = 12, Name = "Bingöl", Districts = new[] { "Adaklı", "Genç", "Karlıova", "Kiğı", "Merkez", "Solhan", "Yayladere", "Yedisu" } },
            new CityItem { Id = 13, Name = "Bitlis", Districts = new[] { "Adilcevaz", "Ahlat", "Güroymak", "Hizan", "Merkez", "Mutki", "Tatvan" } },
            new CityItem { Id = 14, Name = "Bolu", Districts = new[] { "Dörtdivan", "Gerede", "Göynük", "Kıbrıscık", "Mengen", "Merkez", "Mudurnu", "Seben", "Yeniçağa" } },
            new CityItem { Id = 15, Name = "Burdur", Districts = new[] { "Ağlasun", "Altınyayla", "Bucak", "Çavdır", "Çeltikçi", "Gölhisar", "Karamanlı", "Kemer", "Merkez", "Tefenni", "Yeşilova" } },
            new CityItem { Id = 16, Name = "Bursa", Districts = new[] { "Büyükorhan", "Gemlik", "Gürsu", "Harmancık", "İnegöl", "İznik", "Karacabey", "Keles", "Kestel", "Mudanya", "Mustafakemalpaşa", "Nilüfer", "Orhaneli", "Orhangazi", "Osmangazi", "Yenişehir", "Yıldırım" } },
            new CityItem { Id = 17, Name = "Çanakkale", Districts = new[] { "Ayvacık", "Bayramiç", "Biga", "Bozcaada", "Çan", "Eceabat", "Ezine", "Gelibolu", "Gökçeada", "Lapseki", "Merkez", "Yenice" } },
            new CityItem { Id = 18, Name = "Çankırı", Districts = new[] { "Atkaracalar", "Bayramören", "Çerkeş", "Eldivan", "Ilgaz", "Kızılırmak", "Korgun", "Kurşunlu", "Merkez", "Orta", "Şabanözü", "Yapraklı" } },
            new CityItem { Id = 19, Name = "Çorum", Districts = new[] { "Alaca", "Bayat", "Boğazkale", "Dodurga", "İskilip", "Kargı", "Laçin", "Mecitözü", "Merkez", "Oğuzlar", "Ortaköy", "Osmancık", "Sungurlu", "Uğurludağ" } },
            new CityItem { Id = 20, Name = "Denizli", Districts = new[] { "Acıpayam", "Babadağ", "Baklan", "Bekilli", "Beyağaç", "Bozkurt", "Buldan", "Çal", "Çameli", "Çardak", "Çivril", "Güney", "Honaz", "Kale", "Merkezefendi", "Pamukkale", "Sarayköy", "Serinhisar", "Tavas" } },
            new CityItem { Id = 21, Name = "Diyarbakır", Districts = new[] { "Bağlar", "Bismil", "Çermik", "Çınar", "Çüngüş", "Dicle", "Eğil", "Ergani", "Hani", "Hazro", "Kayapınar", "Kocaköy", "Kulp", "Lice", "Silvan", "Sur", "Yenişehir" } },
            new CityItem { Id = 22, Name = "Edirne", Districts = new[] { "Enez", "Havsa", "İpsala", "Keşan", "Lalapaşa", "Meriç", "Merkez", "Süloğlu", "Uzunköprü" } },
            new CityItem { Id = 23, Name = "Elazığ", Districts = new[] { "Ağın", "Alacakaya", "Arıcak", "Baskil", "Karakoçan", "Keban", "Kovancılar", "Maden", "Merkez", "Palu", "Sivrice" } },
            new CityItem { Id = 24, Name = "Erzincan", Districts = new[] { "Çayırlı", "Ilıç", "Kemah", "Kemaliye", "Merkez", "Otlukbeli", "Refahiye", "Tercan", "Üzümlü" } },
            new CityItem { Id = 25, Name = "Erzurum", Districts = new[] { "Aşkale", "Aziziye", "Çat", "Hınıs", "Horasan", "İspir", "Karaçoban", "Karayazı", "Köprüköy", "Narman", "Oltu", "Olur", "Palandöken", "Pasinler", "Pazaryolu", "Şenkaya", "Tekman", "Tortum", "Uzundere", "Yakutiye" } },
            new CityItem { Id = 26, Name = "Eskişehir", Districts = new[] { "Alpu", "Beylikova", "Çifteler", "Günyüzü", "Han", "İnönü", "Mahmudiye", "Mihalgazi", "Mihalıççık", "Odunpazarı", "Sarıcakaya", "Seyitgazi", "Sivrihisar", "Tepebaşı" } },
            new CityItem { Id = 27, Name = "Gaziantep", Districts = new[] { "Araban", "İslahiye", "Karkamış", "Nizip", "Oğuzeli", "Nurdağı", "Şahinbey", "Şehitkamil", "Yavuzeli" } },
            new CityItem { Id = 28, Name = "Giresun", Districts = new[] { "Alucra", "Bulancak", "Çamoluk", "Çanakçı", "Dereli", "Doğankent", "Espiye", "Eynesil", "Görele", "Güce", "Keşap", "Merkez", "Piraziz", "Görele", "Şebinkarahisar", "Tirebolu", "Yağlıdere" } },
            new CityItem { Id = 29, Name = "Gümüşhane", Districts = new[] { "Kelkit", "Köse", "Kürtün", "Merkez", "Şiran", "Torul" } },
            new CityItem { Id = 30, Name = "Hakkari", Districts = new[] { "Çukurca", "Derecik", "Merkez", "Şemdinli", "Yüksekova" } },
            new CityItem { Id = 31, Name = "Hatay", Districts = new[] { "Altınözü", "Antakya", "Arsuz", "Belen", "Defne", "Dörtyol", "Erzin", "Hassa", "İskenderun", "Kırıkhan", "Kumlu", "Payas", "Reyhanlı", "Samandağ", "Yayladağı" } },
            new CityItem { Id = 32, Name = "Isparta", Districts = new[] { "Aksu", "Atabey", "Eğirdir", "Gelendost", "Gönen", "Keçiborlu", "Merkez", "Senirkent", "Sütçüler", "Şarkikaraağaç", "Uluborlu", "Yalvaç", "Yenişarbademli" } },
            new CityItem { Id = 33, Name = "Mersin", Districts = new[] { "Akdeniz", "Anamur", "Aydıncık", "Bozyazı", "Çamlıyayla", "Erdemli", "Gülnar", "Mezitli", "Mut", "Silifke", "Tarsus", "Toroslar", "Yenişehir" } },
            new CityItem { Id = 34, Name = "İstanbul", Districts = new[] { "Adalar", "Arnavutköy", "Ataşehir", "Avcılar", "Bağcılar", "Bahçelievler", "Bakırköy", "Başakşehir", "Bayrampaşa", "Beşiktaş", "Beykoz", "Beylikdüzü", "Beyoğlu", "Büyükçekmece", "Çatalca", "Çekmeköy", "Esenler", "Esenyurt", "Eyüpsultan", "Fatih", "Gaziosmanpaşa", "Güngören", "Kadıköy", "Kağıthane", "Kartal", "Küçükçekmece", "Maltepe", "Pendik", "Sancaktepe", "Sarıyer", "Silivri", "Sultanbeyli", "Sultangazi", "Şile", "Şişli", "Tuzla", "Ümraniye", "Üsküdar", "Zeytinburnu" } },
            new CityItem { Id = 35, Name = "İzmir", Districts = new[] { "Aliağa", "Balçova", "Bayındır", "Bayraklı", "Bergama", "Beydağ", "Bornova", "Buca", "Çeşme", "Çiğli", "Dikili", "Foça", "Gaziemir", "Güzelbahçe", "Karabağlar", "Karaburun", "Karşıyaka", "Kemalpaşa", "Kınık", "Kiraz", "Konak", "Menderes", "Menemen", "Narlıdere", "Ödemiş", "Seferihisar", "Selçuk", "Tire", "Torbalı", "Urla" } },
            new CityItem { Id = 36, Name = "Kars", Districts = new[] { "Akyaka", "Arpaçay", "Digor", "Kağızman", "Merkez", "Sarıkamış", "Selim", "Susuz" } },
            new CityItem { Id = 37, Name = "Kastamonu", Districts = new[] { "Abana", "Ağlı", "Araç", "Azdavay", "Bozkurt", "Cide", "Çatalzeytin", "Daday", "Devrekani", "Doğanyurt", "Hanönü", "İhsangazi", "İnebolu", "Küre", "Merkez", "Pınarbaşı", "Seydiler", "Şenpazar", "Taşköprü", "Tosya" } },
            new CityItem { Id = 38, Name = "Kayseri", Districts = new[] { "Akkışla", "Bünyan", "Develi", "Felahiye", "Hacılar", "İncesu", "Kocasinan", "Melikgazi", "Özvatan", "Pınarbaşı", "Sarıoğlan", "Sarız", "Talas", "Tomarza", "Yahyalı", "Yeşilhisar" } },
            new CityItem { Id = 39, Name = "Kırklareli", Districts = new[] { "Babaeski", "Demirköy", "Kofçaz", "Lüleburgaz", "Merkez", "Pehlivanköy", "Pınarhisar", "Vize" } },
            new CityItem { Id = 40, Name = "Kırşehir", Districts = new[] { "Akçakent", "Akpınar", "Boztepe", "Çiçekdağı", "Kaman", "Merkez", "Mucur" } },
            new CityItem { Id = 41, Name = "Kocaeli", Districts = new[] { "Başiskele", "Çayırova", "Darıca", "Derince", "Dilovası", "Gebze", "Gölcük", "İzmit", "Kandıra", "Karamürsel", "Kartepe", "Körfez" } },
            new CityItem { Id = 42, Name = "Konya", Districts = new[] { "Ahırlı", "Akören", "Akşehir", "Altınekin", "Beyşehir", "Bozkır", "Cihanbeyli", "Çeltik", "Çumra", "Derbent", "Derebucak", "Doğanhisar", "Emirgazi", "Ereğli", "Güneysınır", "Hadim", "Halkapınar", "Hüyük", "Ilgın", "Kadınhanı", "Karapınar", "Karatay", "Kulu", "Meram", "Sarayönü", "Selçuklu", "Seydişehir", "Taşkent", "Tuzlukçu", "Yalıhüyük", "Yunak" } },
            new CityItem { Id = 43, Name = "Kütahya", Districts = new[] { "Altıntaş", "Aslanapa", "Çavdarhisar", "Domaniç", "Dumlupınar", "Emet", "Gediz", "Hisarcık", "Merkez", "Pazarlar", "Simav", "Şaphane", "Tavşanlı" } },
            new CityItem { Id = 44, Name = "Malatya", Districts = new[] { "Akçadağ", "Arapgir", "Arguvan", "Battalgazi", "Darende", "Doğanşehir", "Doğanyol", "Hekimhan", "Kale", "Kuluncak", "Pütürge", "Yazıhan", "Yeşilyurt" } },
            new CityItem { Id = 45, Name = "Manisa", Districts = new[] { "Ahmetli", "Akhisar", "Alaşehir", "Demirci", "Gölmarmara", "Gördes", "Kırkağaç", "Köprübaşı", "Kula", "Salihli", "Sarıgöl", "Saruhanlı", "Selendi", "Soma", "Şehzadeler", "Turgutlu", "Yunusemre" } },
            new CityItem { Id = 46, Name = "Kahramanmaraş", Districts = new[] { "Afşin", "Andırın", "Çağlayancerit", "Dulkadiroğlu", "Ekinözü", "Elbistan", "Göksun", "Nurhak", "Onikişubat", "Pazarcık", "Türkoğlu" } },
            new CityItem { Id = 47, Name = "Mardin", Districts = new[] { "Artuklu", "Dargeçit", "Derik", "Kızıltepe", "Mazıdağı", "Midyat", "Nusaybin", "Ömerli", "Savur", "Yeşilli" } },
            new CityItem { Id = 48, Name = "Muğla", Districts = new[] { "Bodrum", "Dalaman", "Datça", "Fethiye", "Kavaklıdere", "Köyceğiz", "Marmaris", "Menteşe", "Milas", "Ortaca", "Seydikemer", "Ula", "Yatağan" } },
            new CityItem { Id = 49, Name = "Muş", Districts = new[] { "Bulanık", "Hasköy", "Korkut", "Malazgirt", "Merkez", "Varto" } },
            new CityItem { Id = 50, Name = "Nevşehir", Districts = new[] { "Acıgöl", "Avanos", "Derinkuyu", "Gülşehir", "Hacıbektaş", "Kozaklı", "Merkez", "Ürgüp" } },
            new CityItem { Id = 51, Name = "Niğde", Districts = new[] { "Altunhisar", "Bor", "Çamardı", "Çiftlik", "Merkez", "Ulukışla" } },
            new CityItem { Id = 52, Name = "Ordu", Districts = new[] { "Akkuş", "Altınordu", "Aybastı", "Çamaş", "Çatalpınar", "Çaybaşı", "Fatsa", "Gölköy", "Gülyalı", "Gürgentepe", "İkizce", "Kabadüz", "Kabataş", "Korgan", "Kumru", "Mesudiye", "Ünye", "Ulubey" } },
            new CityItem { Id = 53, Name = "Rize", Districts = new[] { "Ardeşen", "Çamlıhemşin", "Çayeli", "Derepazarı", "Fındıklı", "Güneysu", "Hemşin", "İkizdere", "İyidere", "Kalkandere", "Merkez", "Pazar" } },
            new CityItem { Id = 54, Name = "Sakarya", Districts = new[] { "Akyazı", "Arifiye", "Erenler", "Ferizli", "Geyve", "Hendek", "Karapürçek", "Karasu", "Kaynarca", "Kocaali", "Pamukova", "Sapanca", "Serdivan", "Söğütlü", "Taraklı" } },
            new CityItem { Id = 55, Name = "Samsun", Districts = new[] { "19 Mayıs", "Alaçam", "Asarcık", "Atakum", "Bafra", "Canik", "Çarşamba", "Havza", "İlkadım", "Kavak", "Ladik", "Salıpazarı", "Tekkeköy", "Terme", "Vezirköprü", "Yakakent" } },
            new CityItem { Id = 56, Name = "Siirt", Districts = new[] { "Baykan", "Eruh", "Kurtalan", "Merkez", "Pervari", "Şirvan", "Tillo" } },
            new CityItem { Id = 57, Name = "Sinop", Districts = new[] { "Boyabat", "Dikmen", "Durağan", "Erfelek", "Gerze", "Merkez", "Saraydüzü", "Türkeli" } },
            new CityItem { Id = 58, Name = "Sivas", Districts = new[] { "Akıncılar", "Altınyayla", "Divriği", "Doğanşar", "Gemerek", "Gölova", "Gürün", "Hafik", "İmranlı", "Kangal", "Koyulhisar", "Merkez", "Suşehri", "Şarkışla", "Ulaş", "Yıldızeli", "Zara" } },
            new CityItem { Id = 59, Name = "Tekirdağ", Districts = new[] { "Çerkezköy", "Çorlu", "Ergene", "Hayrabolu", "Kapaklı", "Malkara", "Marmaraereğlisi", "Muratlı", "Saray", "Süleymanpaşa", "Şarköy" } },
            new CityItem { Id = 60, Name = "Tokat", Districts = new[] { "Almus", "Artova", "Başçiftlik", "Erbaa", "Merkez", "Niksar", "Pazar", "Reşadiye", "Sulusaray", "Yeşilyurt", "Zile" } },
            new CityItem { Id = 61, Name = "Trabzon", Districts = new[] { "Akçaabat", "Araklı", "Arsin", "Beşikdüzü", "Çarşıbaşı", "Çaykara", "Dernekpazarı", "Düzköy", "Hayrat", "Köprübaşı", "Maçka", "Of", "Ortahisar", "Sürmene", "Şalpazarı", "Tonya", "Vakfıkebir", "Yomra" } },
            new CityItem { Id = 62, Name = "Tunceli", Districts = new[] { "Çemişgezek", "Hozat", "Mazgirt", "Nazımiye", "Ovacık", "Pertek", "Pülümür" } },
            new CityItem { Id = 63, Name = "Şanlıurfa", Districts = new[] { "Akçakale", "Birecik", "Bozova", "Ceylanpınar", "Eyyübiye", "Halfeti", "Haliliye", "Harran", "Hilvan", "Karaköprü", "Siverek", "Suruç", "Viranşehir" } },
            new CityItem { Id = 64, Name = "Uşak", Districts = new[] { "Banaz", "Eşme", "Karahallı", "Merkez", "Sivaslı", "Ulubey" } },
            new CityItem { Id = 65, Name = "Van", Districts = new[] { "Bahçesaray", "Başkale", "Çaldıran", "Çatak", "Edremit", "Erciş", "Gevaş", "Gürpınar", "İpekyolu", "Muradiye", "Özalp", "Saray", "Tuşba" } },
            new CityItem { Id = 66, Name = "Yozgat", Districts = new[] { "Akdağmadeni", "Aydıncık", "Boğazlıyan", "Çandır", "Çayıralan", "Çekerek", "Kadışehri", "Saraykent", "Sarıkaya", "Sorgun", "Şefaatli", "Yenifakılı", "Yerköy" } },
            new CityItem { Id = 67, Name = "Zonguldak", Districts = new[] { "Alaplı", "Çaycuma", "Devrek", "Ereğli", "Gökçebey", "Kilimli", "Kozlu", "Merkez" } },
            new CityItem { Id = 68, Name = "Aksaray", Districts = new[] { "Ağaçören", "Eskil, Gülağaç", "Güzelyurt", "Merkez", "Ortaköy", "Sarıyahşi" } },
            new CityItem { Id = 69, Name = "Bayburt", Districts = new[] { "Aydıntepe", "Demirözü", "Merkez" } },
            new CityItem { Id = 70, Name = "Karaman", Districts = new[] { "Ayrancı", "Başyayla", "Ermenek", "Kazımkarabekir", "Merkez", "Sarıveliler" } },
            new CityItem { Id = 71, Name = "Kırıkkale", Districts = new[] { "Bahşılı", "Balışeyh", "Çelebi", "Delice", "Karakeçili", "Keskin", "Merkez", "Sulakyurt", "Yahşihan" } },
            new CityItem { Id = 72, Name = "Batman", Districts = new[] { "Beşiri", "Gercüş", "Hasankeyf", "Kozluk", "Merkez", "Sason" } },
            new CityItem { Id = 73, Name = "Şırnak", Districts = new[] { "Beytüşşebap", "Cizre", "Güçlükonak", "İdil", "Silopi", "Uludere" } },
            new CityItem { Id = 74, Name = "Bartın", Districts = new[] { "Amasra", "Kurucaşile", "Merkez", "Ulus" } },
            new CityItem { Id = 75, Name = "Ardahan", Districts = new[] { "Çıldır", "Damal", "Göle", "Hanak", "Merkez", "Posof" } },
            new CityItem { Id = 76, Name = "Iğdır", Districts = new[] { "Aralık", "Karakoyunlu", "Merkez", "Tuzluca" } },
            new CityItem { Id = 77, Name = "Yalova", Districts = new[] { "Altınova", "Armutlu", "Çınarcık", "Çiftlikköy", "Merkez", "Termal" } },
            new CityItem { Id = 78, Name = "Karabük", Districts = new[] { "Eflani", "Eskipazar", "Merkez", "Ovacık", "Safranbolu", "Yenice" } },
            new CityItem { Id = 79, Name = "Kilis", Districts = new[] { "Elbeyli", "Merkez", "Musabeyli", "Polateli" } },
            new CityItem { Id = 80, Name = "Osmaniye", Districts = new[] { "Bahçe", "Düziçi", "Hasanbeyli", "Kadirli", "Merkez", "Sumbas", "Toprakkale" } },
            new CityItem { Id = 81, Name = "Düzce", Districts = new[] { "Akçakoca", "Cumayeri", "Çilimli", "Gölyaka", "Gümüşova", "Kaynaşlı", "Merkez", "Yığılca" } }
        };
    }
}