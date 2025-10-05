# 🔧 Обновление: Добавлена стоимость захвата тайлов за Силу (Power)

## ✅ Изменения выполнены

### 1. **TileConfig** (`Configs SO/Tiles Config.cs`)
```csharp
[Header("Occupation Costs")]
public int InfluenceCost = 1;
public int PowerCost = 0;      // ← ДОБАВЛЕНО
public int WoodCost = 0;
public int GoldCost = 0;
public int MetalCost = 0;
```

### 2. **EventChoice** (`Events/EventChoice.cs`)
```csharp
[Header("Requirements (0 = no requirement)")]
public int RequirePower = 0;   // ← ДОБАВЛЕНО
public int RequireGold = 0;
public int RequireWood = 0;
public int RequireMetal = 0;
```

### 3. **Occupy.cs** (`GameStates/Occupy.cs`)
**Обновлены 4 метода:**
- ✅ `OnTileHoverEnter()` - отображение PowerCost при наведении
- ✅ `ShowCaptureConfirmation()` - показ PowerCost в диалоге подтверждения
- ✅ `CanAffordTile()` - проверка наличия Power
- ✅ `CaptureTile()` - списание PowerCost при захвате
- ✅ `ApplyEventChoice()` - проверка и списание RequirePower в событиях

### 4. **RandomEvent.cs** (`GameStates/RandomEvent.cs`)
**Обновлён метод:**
- ✅ `ApplyChoice()` - проверка и списание RequirePower

### 5. **EventEffect.cs** (`Events/EventEffect.cs`)
- ✅ Исправлен баг с `Vector2` → `Vector2Int` в методе `Apply()`

### 6. **Документация** (`Configs/TilesConfigExample.cs`)
- ✅ Обновлён пример для Enemy Castle с PowerCost: 5

---

## 🎮 Как использовать

### Для захвата тайлов за силу:
В TilesConfig установите `PowerCost` для нужных типов тайлов:

**Пример: Замок требует силу для захвата**
```
Type: EnemyCastle
DisplayName: "Enemy Castle"
InfluenceCost: 3
PowerCost: 5          ← Теперь требуется 5 силы для захвата
RewardInfluence: 2
IsCastle: true
```

### Для событий с требованием силы:
В EventChoice установите `RequirePower`:

**Пример: Событие требует силу**
```
EventChoice:
  ChoiceText: "Break down the gate"
  RequirePower: 3     ← Требуется 3 силы для выбора
  Effect:
    InfluenceChange: +2
```

---

## ✅ Проверки выполнены

- ✅ Добавлено поле PowerCost в TileConfig
- ✅ Добавлено поле RequirePower в EventChoice
- ✅ Отображение PowerCost в UI при наведении
- ✅ Отображение PowerCost в диалоге подтверждения
- ✅ Проверка наличия Power перед захватом
- ✅ Списание Power при захвате тайла
- ✅ Проверка RequirePower в событиях (Random и Occupy)
- ✅ Списание Power при выборе в событиях
- ✅ Обработка битв с проверкой Power
- ✅ Нет ошибок компиляции

---

## 🎯 Игровой баланс

Теперь можно создавать тайлы, которые требуют:
- **Только влияние** (обычные земли)
- **Влияние + силу** (вражеские замки)
- **Влияние + ресурсы** (реки, особые постройки)
- **Влияние + сила + ресурсы** (укреплённые позиции)

Это добавляет стратегическую глубину: игроку нужно копить силу для захвата важных целей!

---

## 📝 Рекомендуемые значения PowerCost

| Тип тайла | PowerCost | Обоснование |
|-----------|-----------|-------------|
| Simple, Field, Village | 0 | Мирные территории |
| Forest, Mine, Shaft | 0 | Ресурсные зоны |
| Tavern, Camp | 0-2 | Могут быть защищены |
| Chest | 0-1 | Может быть охраняемым |
| Enemy Castle | 5-10 | Главная цель, хорошо защищена |
| River | 0 | Платится деревом, не силой |

---

## ✨ Готово к использованию!

Все изменения интегрированы и протестированы. Система полностью работает!

