# 🚀 Quick Start - Procedural Map Generator

## За 5 минут до первой карты!

### Метод 1: Setup Wizard (Рекомендуется)

1. **Unity Menu** → `Tools` → `Map Generator` → `Setup Wizard`

2. В открывшемся окне:
   - **Config Name**: `MyFirstMap`
   - **Map Radius**: `5` (91 тайл)
   - **Preset**: `Balanced`
   - Нажмите **Create Configuration**

3. Создайте пустой GameObject в сцене:
   - Назовите его `MapGenerator`
   - Добавьте компонент `MapGeneratorController`

4. Назначьте references в инспекторе:
   - **MapSettings**: перетащите ваш MapSettings asset
   - **MapView**: перетащите MapView из сцены
   - **Generation Config**: перетащите созданный `MyFirstMap` config

5. Назначьте префабы тайлов (из `Assets/Prefabs/Tiles/`):
   - Simple Prefab → `Simple.prefab`
   - Field Prefab → `Field.prefab`
   - Village Prefab → `Vilage.prefab`
   - Enemy Castle Prefab → `Enemy Castle.prefab`
   - Player Castle Prefab → `Player Castle.prefab`
   - Forest Prefab → `Forest.prefab`
   - Mine Prefab → `Mine.prefab`
   - Shaft Prefab → `Shaft.prefab`
   - Chest Prefab → `Chest.prefab`
   - Tavern Prefab → `Tavern.prefab`
   - Camp Prefab → `Camp.prefab`
   - River Prefab → `River.prefab`
   - Mountain Prefab → `Mountain.prefab`

6. **Нажмите кнопку "Generate New Map"** в инспекторе

✅ **Готово!** Карта сгенерирована!

---

### Метод 2: Ручное создание

1. **ПКМ** в Project → `Create` → `Configs` → `Map Generation Config`

2. В конфиге:
   - Установите `Map Radius = 5`
   - Настройте проценты так, чтобы сумма = 100%
   - Или нажмите кнопку **Balanced** для автоматической настройки

3. Следуйте шагам 3-6 из Метода 1

---

## 🎮 Управление

### Горячие клавиши (если добавлен QuickStartExample)
- **R** - Регенерировать карту с новым seed
- **T** - Показать статистику карты

### Кнопки в инспекторе
- **Generate New Map** - Сгенерировать карту
- **Random Seed** - Новый случайный seed
- **Show Stats** - Статистика и анализ
- **Clear Map** - Очистить все тайлы

---

## 🎯 Готовые пресеты

В инспекторе MapGenerationConfig нажмите:

| Пресет | Описание |
|--------|----------|
| **Balanced** | Сбалансированная карта для нормальной игры |
| **Resource Rich** | Много ресурсов, легкий режим |
| **Challenging** | Мало ресурсов, много препятствий |
| **Exploration** | Фокус на исследование, много событий |
| **Combat Focus** | Много врагов, боевой режим |

---

## 📊 Анализ карты

После генерации нажмите **Show Stats** чтобы увидеть:
- ✅ Общее количество тайлов
- ✅ Распределение по типам
- ✅ Плотность ресурсов
- ✅ Предупреждения о проблемах
- ✅ Рекомендации по улучшению

---

## 🔧 Частые настройки

### Изменить размер карты
```
Map Radius:
1 = 7 тайлов (крошечная)
3 = 37 тайлов (маленькая)
5 = 91 тайл (средняя, рекомендуется)
7 = 169 тайлов (большая)
10 = 331 тайл (огромная)
```

### Изменить сложность
```
Danger Progression:
0.0 = равномерная сложность
0.5 = умеренный рост (рекомендуется)
1.0 = максимальная опасность на краях
```

### Зафиксировать карту
Чтобы всегда генерировать одну и ту же карту:
1. Сгенерируйте карту
2. Посмотрите в консоль: `Generating map with seed: 12345`
3. Установите `Seed = 12345` в конфиге
4. Теперь карта будет одинаковой при каждой генерации

---

## ❗ Troubleshooting

**"Cannot generate map - missing references!"**
→ Назначьте MapSettings, MapView и GenerationConfig

**"No prefab registered for [TileType]"**
→ Назначьте все префабы в MapGeneratorController

**Тайлы накладываются друг на друга**
→ Проверьте что MapView.Tiles очищен перед генерацией

**Карта несбалансирована**
→ Нажмите Show Stats и следуйте рекомендациям

---

## 💡 Следующие шаги

1. **Поэкспериментируйте** с разными пресетами
2. **Настройте проценты** под свою игру
3. **Найдите удачный seed** и сохраните его
4. **Добавьте QuickStartExample** для генерации по нажатию R
5. **Прочитайте README.md** для продвинутых возможностей

---

## 📚 Больше информации

- **Полная документация**: `README.md`
- **История изменений**: `CHANGELOG.md`
- **Примеры кода**: `Examples/QuickStartExample.cs`

---

**Приятной генерации! 🎲**

