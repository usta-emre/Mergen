# Mergen

Mergen, Microsoft IIS (Internet Information Services) servisi ile yayın yapan ASP.NET uygulamalarına gelen ağ paketlerini loglayarak trafiklerin analizini kolaylaştırmayı sağlayan bir projedir.

Loglama işlemi konfigürasyon dosyasında belirtilen üç farklı kural ile yapılabilir:

	1- Gelen bütün web trafikleri loglar 
	2- Belirli bir Source IP üzerinden gelen web trafikleri loglar
	3- Belirli bir URL uzantısına gelen web trafikleri loglar

## Kurulum:

- Microsoft.MergenModules.dll ve mergen.xml dosyalarının proje dizininde bulunan bin klasörüne kopyalanmalıdır.

- Mergen modulü, proje dizininde bulunan web.config dosyasına aşağıdaki örnekteki gibi eklenmelidir.

    <system.webServer>
      <modules>
        <add name="MergenLogger" type="Microsoft.MergenModules.MergenLogger, Microsoft.MergenModules" />
      </modules>
    </system.webServer>
  
- Hata alınması durumunda Process Monitor (Procmon) uygulaması ile w3wp uygulamasının yaptığı aktiviteler analiz edilebilir.

## Loglama ayarları:

- Bütün trafiklerin loglanması:

Uygulamaya gelen bütün web trafiklerin loglanması için mergen.xml dosyası içerisine aşağıdaki örnekteki gibi 'type="All"' değeri eklenmelidir.
Method değeri ile loglanması istenilen HTTP metodu seçilebilir. (Default: All)
Response değeri true ise trafik responselarının body kısmını da loglar. (Default: true)

Örneğin:

    <?xml version="1.0" encoding="UTF-8" ?>
    <mergen>
      <listener>
        <url type="All" response="true" method="All"></url>
      </listener>
     </mergen>  

- Belirli bir Source IP'den gelen trafiklerin loglanması:

Uygulamaya belirli bir source IP den gelen web trafiklerin loglanması için mergen.xml dosyası içerisine aşağıdaki örnekteki gibi 'type="IP"' değeri eklenmelidir.
Method değeri ile loglanılması istenilen HTTP metodu seçilebilir. (Default: All)
Response değeri true ise trafik responselarının body kısmını da loglar. (Default: true)

Örneğin:

    <?xml version="1.0" encoding="UTF-8" ?>
    <mergen>
      <listener>
        <url type="IP" response="false" method="POST">192.168.14.1</url>
      </listener>
     </mergen>  
     
- Belirli bir URL uzantısına gelen trafiklerin loglanması:

Uygulamanın belli bir URL uzantısına gelen web trafiklerin loglanması için mergen.xml dosyası içerisine aşağıdaki örnekteki gibi 'type="Path"' değeri eklenmelidir.
Method değeri ile loglanmasını istediğiniz HTTP methodunu seçebilirsiniz. (Default: All)
Response değeri true ise response trafiklerdeki body kısmınıda loglar. (Default: true)

Örneğin:

    <?xml version="1.0" encoding="UTF-8" ?>
    <mergen>
      <listener>
        <url type="Path" response="true" method="GET">404.aspx</url>
      </listener>
     </mergen> 

## Şüpheli trafik logları:

Proje Localsystem yetkisinde çalışıyor ise şüpheli trafiklere Windows Event Log dizininde bulunan Microsoft-IIS-MergenLog event logundan ulaşabilirsiniz.
Eğer proje farklı bir yetkide çalışıyor ise şüpheli trafiklere Windows Event Log dizininde bulunan Application event logu içerisindeki 8875 event idsi ile ulaşabilirsiniz.
Bir hata alırsanız hatanın detaylarına Windows Event Log dizininde bulunan Application event logu içerisindeki Error türünde 8875 event idsi ile ulaşabilirsiniz.


Proje Localsystem yetkisinde çalışıyor ise şüpheli web trafiklere Windows Event Log dizininde bulunan Microsoft-IIS-MergenLog event logundan ulaşabilirsiniz. Eğer proje farklı bir yetkide çalışıyor ise şüpheli trafiklere Windows Event Log dizininde bulunan Application event logu içerisindeki 8875 event idsi ile ulaşabilirsiniz.
Bir hata alınması durumunda hatanın detaylarına Windows Event Log dizininde bulunan Application event logu içerisindeki Error türünde 8875 event idsi ile ulaşabilirsiniz.

Demo [kaydı](https://youtu.be/pTvdWe7tT5U)
