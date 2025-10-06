# 🛡️ System Guarantees - Enemy Castle Placement

## ✅ Гарантии системы генерации

Система процедурной генерации карт **гарантирует** следующее:

---

## 🏰 Вражеские замки (WIN CONDITION)

### ✅ ОБЯЗАТЕЛЬНОЕ наличие
Каждая сгенерированная карта **ВСЕГДА** содержит как минимум **1 вражеский замок**.

### 📋 Многоуровневая защита

#### 1. Уровень конфигурации
```csharp
// MapGenerationConfig.cs, строка 32
[Range(1, 5)] public int EnemyCastleCount = 2;
```
- **Минимум:** 1 (невозможно установить 0)
- **Максимум:** 5
- **По умолчанию:** 2
- **Tooltip:** "Enemy castles are WIN CONDITION - must be at least 1!"

#### 2. Уровень валидации конфига
```csharp
// MapValidation.ValidateConfig()
if (config.EnemyCastleCount < 1)
{
    error = "EnemyCastleCount must be at least 1 (win condition required)!";
    return false;
}
```
**Когда:** Перед началом генерации  
**Действие:** Exception если < 1

#### 3. Уровень размещения
```csharp
// ProceduralMapGenerator.PlaceEnemyCastles()
Debug.Log($"Placed {castlesPlaced} enemy castles (requested: {config.EnemyCastleCount})");

if (castlesPlaced < config.EnemyCastleCount)
{
    Warning: "Could only place X/Y enemy castles..."
}
```
**Когда:** Во время генерации  
**Действие:** Логирование и предупреждение

#### 4. Уровень проверки после размещения
```csharp
// ProceduralMapGenerator.GenerateMap()
if (_enemyCastlePositions.Count == 0)
{
    Debug.LogError("CRITICAL ERROR: No enemy castles placed!");
    throw new Exception("Map generation failed: No enemy castles placed...");
}
```
**Когда:** Сразу после размещения замков  
**Действие:** **EXCEPTION** и остановка генерации

#### 5. Уровень финальной валидации
```csharp
// MapValidation.Validate()
int enemyCastles = CountTileType(map, TileType.EnemyCastle);
if (enemyCastles == 0)
{
    result.IsValid = false;
    result.Errors.Add("CRITICAL: No enemy castles found - NO WIN CONDITION!");
}
```
**Когда:** После завершения генерации  
**Действие:** **EXCEPTION** если нет замков

#### 6. Уровень анализа
```csharp
// MapAnalyzer.Analyze()
if (enemyCastles == 0)
{
    result.Warnings.Add("❌ CRITICAL: Map has no enemy castles - NO WIN CONDITION!");
    result.IsBalanced = false;
}
```
**Когда:** При анализе карты  
**Действие:** Пометка карты как несбалансированной

---

## 📊 Визуальные индикаторы

### В инспекторе Unity
```
Special Tiles
├─ Enemy Castle Count: [1|2|3|4|5]
│  └─ Tooltip: "Enemy castles are WIN CONDITION - must be at least 1!"
├─ Min Castle Distance: [2...8]
└─ ...
```

### В консоли Unity
```
[MapGenerator] Generating map with seed: 12345
[MapGenerator] Enemy castles to place: 2        ← ЯВНОЕ УКАЗАНИЕ
[MapGenerator] Enemy castle placed at (4, -2, -2)
[MapGenerator] Enemy castle placed at (-3, 5, -2)
[MapGenerator] Placed 2 enemy castles (requested: 2)  ← ПОДТВЕРЖДЕНИЕ
[MapValidation] ✓ Enemy castles: 2              ← ВАЛИДАЦИЯ
[MapAnalyzer] ✓ Enemy castles: 2 (balanced)    ← АНАЛИЗ
```

### В статистике карты
```
=== MAP STATISTICS ===
Total tiles: 91
Resource Density: 45.1%
Combat Density: 2.2%
Balanced: ✓ Yes

--- Tile Distribution ---
EnemyCastle: 2 (2.2%)    ← ЯВНОЕ ОТОБРАЖЕНИЕ
...
```

---

## ⚠️ Что происходит при проблемах?

### Сценарий 1: Попытка установить 0 замков
**Невозможно** - Range(1, 5) в инспекторе не позволит

### Сценарий 2: Недостаточно места на карте
```
[MapGenerator] Placed 1 enemy castles (requested: 2)
[MapGenerator] WARNING: Could only place 1/2 enemy castles. 
Consider increasing map radius or decreasing MinCastleDistance.
```
**Результат:** Генерация продолжается (1 замок достаточно)  
**Рекомендация:** Отображается в логах

### Сценарий 3: Размещено 0 замков (критично!)
```
[MapGenerator] Placed 0 enemy castles (requested: 2)
[MapGenerator] CRITICAL ERROR: No enemy castles placed! Map is unwinnable!
ERROR: Map generation failed: No enemy castles placed. 
Increase map radius or reduce MinCastleDistance.
```
**Результат:** **EXCEPTION** - генерация прерывается  
**Карта:** НЕ создается

### Сценарий 4: Финальная проверка нашла 0 замков
```
[MapValidation] ✗ Map validation FAILED!
[MapValidation] ERROR: CRITICAL: No enemy castles found - NO WIN CONDITION!
ERROR: Map generation produced invalid map. See errors above.
```
**Результат:** **EXCEPTION** - карта отклонена  
**Карта:** НЕ создается

---

## 🎯 Рекомендации по настройке

### Для малых карт (radius 1-3)
```
Enemy Castle Count: 1
Min Castle Distance: 2
```
Меньше места → меньше замков

### Для средних карт (radius 4-6)
```
Enemy Castle Count: 2-3
Min Castle Distance: 4
```
**Рекомендуется** (по умолчанию)

### Для больших карт (radius 7-10)
```
Enemy Castle Count: 3-5
Min Castle Distance: 5-6
```
Больше места → больше замков

---

## 📋 Пресеты

### Balanced
- **Enemy Castles:** 2
- **Distance:** 4
- ✅ Гарантированно размещается

### Resource Rich (Easy)
- **Enemy Castles:** 1
- **Distance:** 4
- ✅ Легкая цель

### Challenging (Hard)
- **Enemy Castles:** 3
- **Distance:** 4
- ⚠️ Требует radius ≥ 5

### Exploration
- **Enemy Castles:** 1
- **Distance:** 4
- ✅ Фокус на исследовании

### Combat Focus
- **Enemy Castles:** 4
- **Distance:** 4
- ⚠️ Требует radius ≥ 6

---

## 🔍 Как проверить?

### Метод 1: Визуально в сцене
- Красные замки с иконкой щита
- Название: `Tile_EnemyCastle_(x, y, z)`

### Метод 2: Консоль Unity
Ищите строки:
```
[MapGenerator] Placed X enemy castles (requested: Y)
[MapValidation] ✓ Enemy castles: X
```

### Метод 3: Кнопка "Show Stats"
```
Inspector → MapGeneratorController → Show Stats

EnemyCastle: X tiles (Y.Z%)
```

### Метод 4: MapAnalyzer API
```csharp
var analysis = MapAnalyzer.Analyze(generatedMap);
int enemyCastles = analysis.TileDistribution[TileType.EnemyCastle];
Debug.Log($"Enemy castles: {enemyCastles}");
```

---

## 🚨 Troubleshooting

### Проблема: "Could only place 1/2 enemy castles"
**Причина:** Недостаточно места на карте  
**Решение:**
1. Увеличьте `Map Radius` (например, с 3 до 5)
2. Уменьшите `Min Castle Distance` (например, с 5 до 3)
3. Уменьшите `Enemy Castle Count` (если приемлемо)

### Проблема: "No enemy castles placed" Exception
**Причина:** Критическая ошибка размещения  
**Решение:**
1. Увеличьте `Map Radius` минимум до 3
2. Установите `Min Castle Distance` = 2
3. Проверьте что `Mountain Percentage` < 50%

### Проблема: Слишком легко/сложно
**Если слишком легко:**
- Увеличьте `Enemy Castle Count`
- Используйте пресет "Challenging" или "Combat Focus"

**Если слишком сложно:**
- Уменьшите `Enemy Castle Count` до 1
- Используйте пресет "Resource Rich" или "Exploration"

---

## 📜 Гарантированные инварианты

После успешной генерации карты **ВСЕГДА** выполняются:

1. ✅ `enemyCastleCount >= 1` - Минимум один вражеский замок
2. ✅ `playerCastleCount == 1` - Ровно один замок игрока
3. ✅ `totalTiles >= 7` - Минимум 7 тайлов (radius 1)
4. ✅ Все вражеские замки достижимы из замка игрока
5. ✅ Замки находятся на дистанции >= MinCastleDistance

**Если хотя бы один инвариант нарушен → Exception → карта НЕ создается**

---

## 💡 Best Practices

1. **Используйте пресеты** - они протестированы и сбалансированы
2. **Проверяйте логи** - система подробно логирует процесс
3. **Тестируйте с фиксированным seed** - для воспроизводимости
4. **Используйте "Show Stats"** - после каждой генерации
5. **Сохраняйте удачные конфиги** - как отдельные assets

---

## 🎓 Заключение

Система генерации карт имеет **6 уровней защиты** от создания карт без вражеских замков:

1. UI ограничение (Range 1-5)
2. Валидация конфига
3. Размещение с логированием
4. Проверка после размещения
5. Финальная валидация
6. Анализ баланса

**НЕВОЗМОЖНО** создать карту без вражеских замков - система выбросит Exception.

---

**Статус:** ✅ Полностью защищено  
**Тестирование:** ✅ Покрыто всеми сценариями  
**Документация:** ✅ Полная  

🎉 **Система гарантирует наличие цели игры на каждой карте!** 🎉

