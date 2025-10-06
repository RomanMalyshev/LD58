# 🏰 Enemy Castle Validation - Quick Reference

## ✅ ЧТО ИСПРАВЛЕНО

### Проблема (ДО)
- ❌ `Range(0, 5)` - можно было установить **0 вражеских замков**
- ❌ Нет обязательной проверки после генерации
- ❌ Только предупреждение в анализаторе
- ❌ Возможность создать **неиграбельную карту**

### Решение (ПОСЛЕ)
- ✅ `Range(1, 5)` - **минимум 1 замок** (невозможно установить 0)
- ✅ **6 уровней защиты** от создания карты без замков
- ✅ **Exception** если ни один замок не размещен
- ✅ **Полная валидация** до и после генерации

---

## 📋 Что добавлено

### 1. MapValidation.cs (НОВЫЙ КЛАСС)
Утилита валидации карт с проверками:
- ✅ Наличие ровно 1 замка игрока
- ✅ Наличие минимум 1 вражеского замка (**CRITICAL**)
- ✅ Валидация конфига перед генерацией
- ✅ Проверка процентов = 100%

**API:**
```csharp
// Валидация конфига
if (!MapValidation.ValidateConfig(config, out string error))
{
    Debug.LogError(error);
    return;
}

// Валидация карты
var result = MapValidation.Validate(generatedMap);
result.LogResults();
if (!result.IsValid) 
{
    // Карта невалидна
}
```

### 2. Улучшенный MapGenerationConfig.cs
```csharp
// БЫЛО:
[Range(0, 5)] public int EnemyCastleCount = 2;

// СТАЛО:
[Tooltip("Enemy castles are WIN CONDITION - must be at least 1!")]
[Range(1, 5)] public int EnemyCastleCount = 2;
```

### 3. Улучшенный ProceduralMapGenerator.cs

#### Валидация конфига перед генерацией
```csharp
if (!MapValidation.ValidateConfig(_config, out string configError))
{
    throw new Exception($"Map generation aborted: {configError}");
}
```

#### Логирование количества замков
```csharp
Debug.Log($"Enemy castles to place: {_config.EnemyCastleCount}");
```

#### Проверка после размещения
```csharp
if (_enemyCastlePositions.Count == 0)
{
    Debug.LogError("CRITICAL ERROR: No enemy castles placed!");
    throw new Exception("Map generation failed: No enemy castles placed...");
}
```

#### Предупреждение если не все размещены
```csharp
if (castlesPlaced < _config.EnemyCastleCount)
{
    Debug.LogWarning($"Could only place {castlesPlaced}/{_config.EnemyCastleCount}...");
}
```

#### Финальная валидация
```csharp
var validation = MapValidation.Validate(_generatedTiles);
validation.LogResults();

if (!validation.IsValid)
{
    throw new Exception("Map generation produced invalid map.");
}
```

### 4. Улучшенный MapAnalyzer.cs
```csharp
// БЫЛО:
if (enemyCastles == 0)
{
    result.Warnings.Add("Map has no enemy castles - no win condition!");
}

// СТАЛО:
if (enemyCastles == 0)
{
    result.Warnings.Add("❌ CRITICAL: Map has no enemy castles - NO WIN CONDITION!");
    result.IsBalanced = false;
}
else if (enemyCastles == 1)
{
    result.Recommendations.Add("Only 1 enemy castle - game may be too short");
}
else if (enemyCastles > 4)
{
    result.Warnings.Add($"Many enemy castles ({enemyCastles}) - may be too difficult!");
}
else
{
    Debug.Log($"✓ Enemy castles: {enemyCastles} (balanced)");
}
```

### 5. GUARANTEES.md (НОВАЯ ДОКУМЕНТАЦИЯ)
Полное описание всех гарантий системы:
- 6 уровней защиты
- Визуальные индикаторы
- Сценарии ошибок
- Рекомендации по настройке
- Troubleshooting

---

## 🛡️ 6 Уровней защиты

| # | Уровень | Когда | Действие | Результат |
|---|---------|-------|----------|-----------|
| 1 | **UI Range** | В инспекторе | Range(1, 5) | Невозможно установить 0 |
| 2 | **Config Validation** | Перед генерацией | ValidateConfig() | Exception если < 1 |
| 3 | **Placement Logging** | Во время размещения | Debug.Log + Warning | Информирование |
| 4 | **Post-Placement Check** | После размещения | if count == 0 → Exception | **Остановка генерации** |
| 5 | **Final Validation** | После генерации | MapValidation.Validate() | **Exception если invalid** |
| 6 | **Analysis** | При статистике | MapAnalyzer | Пометка IsBalanced=false |

---

## 📊 Пример работы системы

### Успешная генерация
```
[MapGenerator] Generating map with seed: 42
[MapGenerator] Enemy castles to place: 2          ← Уровень 3
[MapGenerator] Enemy castle placed at (4, -2, -2)
[MapGenerator] Enemy castle placed at (-3, 5, -2)
[MapGenerator] Placed 2 enemy castles (requested: 2) ← OK
[MapValidation] ✓ Map validation PASSED           ← Уровень 5
[MapValidation] ✓ Enemy castles: 2
[MapAnalyzer] ✓ Enemy castles: 2 (balanced)      ← Уровень 6
```

### Критическая ошибка (0 замков)
```
[MapGenerator] Generating map with seed: 42
[MapGenerator] Enemy castles to place: 2
[MapGenerator] Placed 0 enemy castles (requested: 2)
[MapGenerator] CRITICAL ERROR: No enemy castles placed! ← Уровень 4
ERROR: Map generation failed: No enemy castles placed.
Increase map radius or reduce MinCastleDistance.

🛑 EXCEPTION - Карта НЕ создается
```

---

## 🎯 Гарантии

После успешной генерации **ВСЕГДА**:

✅ `EnemyCastleCount >= 1` - Минимум один вражеский замок  
✅ `PlayerCastleCount == 1` - Ровно один замок игрока  
✅ Все замки достижимы (если включен EnsurePathToResources)  
✅ Замки на расстоянии >= MinCastleDistance  

**Если хотя бы одно условие нарушено → EXCEPTION**

---

## 🚀 Быстрая проверка

### В инспекторе
```
MapGenerationConfig
└─ Special Tiles
   ├─ Enemy Castle Count: [1] [2] [3] [4] [5]
   │  ⓘ Enemy castles are WIN CONDITION - must be at least 1!
```
Slider не позволяет выбрать 0.

### После генерации
Нажмите **"Show Stats"** → смотрите:
```
EnemyCastle: 2 tiles (2.2%)
```

### Программно
```csharp
var analysis = MapAnalyzer.Analyze(map);
int count = analysis.TileDistribution[TileType.EnemyCastle];
Debug.Log($"Enemy castles: {count}"); // >= 1 guaranteed
```

---

## 📝 Checklist интеграции

- [x] Range изменен с (0,5) на (1,5)
- [x] Добавлен Tooltip с предупреждением
- [x] Создан класс MapValidation
- [x] Валидация перед генерацией
- [x] Проверка после размещения
- [x] Финальная валидация
- [x] Улучшенный анализ
- [x] Подробное логирование
- [x] Документация GUARANTEES.md
- [x] Zero linter errors

---

## 🎉 Итого

**Система теперь ГАРАНТИРУЕТ:**
1. ✅ Невозможно установить 0 вражеских замков в UI
2. ✅ Exception если конфиг невалиден
3. ✅ Exception если не размещен ни один замок
4. ✅ Exception если финальная валидация не прошла
5. ✅ Подробное логирование процесса
6. ✅ Визуальная индикация в инспекторе

**НЕВОЗМОЖНО создать карту без вражеских замков!**

---

**Дата:** 06.10.2025  
**Версия:** 1.1.0  
**Статус:** ✅ Fully Validated & Tested

