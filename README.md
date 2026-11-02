# SeriDoviz (mobil)

Canlı altın ve döviz kurlarını gösteren, döviz çevirici içeren .NET MAUI mobil uygulaması (Android / iOS).

Kurlar [SeriGold](https://github.com/Oguzhancilesiz/SeriGold) backend'inden REST ile çekilir, SignalR ile anlık güncellenir.

## Ekranlar

- **Altın:** gram, çeyrek, yarım vb. alış/satış fiyatları ve değişim yüzdesi
- **Döviz:** başlıca kurlar, yükseliş/düşüşe göre renklendirme
- **Çevirici:** kurlar arasında anlık dönüşüm

## Teknolojiler

- .NET MAUI
- MVVM: CommunityToolkit.Mvvm (`ObservableObject`, `RelayCommand`)
- Refit ile tip güvenli REST istemcisi (`ISeriDovizApi`)
- SignalR Client ile gerçek zamanlı kur güncellemeleri
- Value converter'lar (değişime göre renk, yüzde formatı)

## Yapı

```
SeriDoviz/
  Services/    → ISeriDovizApi (Refit), RatesService
  ViewModels/  → AltinViewModel, DovizViewModel, CeviriciViewModel
  Views/       → AltinPage, DovizPage, CeviriciPage
  Converters/  → ChangeToColor, PercentText, NullToBool
```

## Çalıştırma

```bash
git clone https://github.com/Oguzhancilesiz/SeriDovizaPP.git
cd SeriDovizaPP
dotnet build SeriDoviz/SeriDoviz.csproj -t:Run -f net9.0-android
```
