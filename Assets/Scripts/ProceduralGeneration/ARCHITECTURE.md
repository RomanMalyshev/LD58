# Architecture - Procedural Map Generator

## 🏗️ Архитектурный обзор

Система построена по модульному принципу с четким разделением ответственности.

```
┌─────────────────────────────────────────────────────────────┐
│                    Unity Editor / Runtime                    │
└─────────────────────────────────────────────────────────────┘
                              ↓
┌─────────────────────────────────────────────────────────────┐
│              MapGeneratorController (Entry Point)            │
│  • Управляет процессом генерации                            │
│  • Регистрирует префабы                                      │
│  • Предоставляет публичный API                               │
└─────────────────────────────────────────────────────────────┘
                ↓                           ↓
┌───────────────────────────┐   ┌─────────────────────────────┐
│ ProceduralMapGenerator    │   │    MapBuilder               │
│ • Генерирует данные       │   │ • Строит физическую карту   │
│ • Создает сетку           │→  │ • Инстанцирует префабы      │
│ • Размещает тайлы         │   │ • Сохраняет в MapSettings   │
│ • Обеспечивает баланс     │   │                             │
└───────────────────────────┘   └─────────────────────────────┘
                ↓                           ↓
┌───────────────────────────┐   ┌─────────────────────────────┐
│ MapGenerationConfig       │   │    MapAnalyzer              │
│ • Параметры генерации     │   │ • Анализирует баланс        │
│ • Процентное распред.     │   │ • Вычисляет метрики         │
│ • Правила и ограничения   │   │ • Дает рекомендации         │
└───────────────────────────┘   └─────────────────────────────┘
```

## 📦 Компоненты и ответственность

### 1. Configuration Layer (Слой конфигурации)

#### MapGenerationConfig (ScriptableObject)
**Ответственность:** Хранение параметров генерации

**Данные:**
- Размер карты (радиус)
- Процентное распределение типов тайлов
- Количество специальных объектов
- Правила генерации (связность, кластеризация)
- Настройки баланса (centralization, danger progression)

**Методы:**
- `GetRandomTileType()` - Выбор типа тайла по вероятностям
- `OnValidate()` - Валидация параметров

**Жизненный цикл:** Asset, создается в редакторе, переиспользуется

---

### 2. Generation Layer (Слой генерации)

#### ProceduralMapGenerator (Class)
**Ответственность:** Создание данных карты

**Процесс генерации:**
```
1. Инициализация Random с seed
2. Генерация гексагональной сетки (cube coordinates)
3. Размещение замка игрока (центр)
4. Размещение вражеских замков (с дистанцией)
5. Размещение специальных тайлов (chest, tavern, camp)
6. Генерация ландшафта и ресурсов
7. Обеспечение связности (pathfinding)
8. Пост-обработка (кластеризация)
```

**Алгоритмы:**
- **Grid Generation:** Cube coordinate iteration
- **Placement:** Distance-based filtering + random selection
- **Terrain:** Weighted random with distance modifiers
- **Connectivity:** BFS с созданием путей
- **Clustering:** Neighbor propagation (30% chance)

**Вход:**
- MapGenerationConfig
- MapSettings

**Выход:**
- `Dictionary<Vector3Int, TileType>` - Карта позиций → типов

**Жизненный цикл:** Создается для каждой генерации

---

### 3. Building Layer (Слой построения)

#### MapBuilder (Class)
**Ответственность:** Построение физической карты из данных

**Процесс:**
```
1. Регистрация префабов для каждого типа
2. Очистка существующих тайлов
3. Для каждого тайла:
   - Конвертация позиции в world space
   - Инстанцирование префаба
   - Настройка компонента Tile
4. (Опционально) Сохранение в MapSettings
```

**Методы:**
- `RegisterTilePrefab()` - Регистрация префаба
- `BuildMap()` - Построение карты
- `SaveToMapSettings()` - Сохранение данных
- `ClearExistingTiles()` - Очистка

**Вход:**
- `Dictionary<Vector3Int, TileType>` из Generator
- Префабы тайлов
- MapSettings
- Parent Transform

**Выход:**
- Физическая карта в сцене
- (Опционально) Обновленный MapSettings asset

**Жизненный цикл:** Создается для каждой генерации

---

### 4. Controller Layer (Слой управления)

#### MapGeneratorController (MonoBehaviour)
**Ответственность:** Координация процесса и интеграция с Unity

**Функции:**
- Хранение ссылок на все необходимые компоненты
- Регистрация префабов
- Запуск генерации
- Предоставление публичного API
- Интеграция с Unity lifecycle

**Публичные методы:**
- `GenerateNewMap()` - Основной метод генерации
- `GenerateWithRandomSeed()` - Генерация с случайным seed
- `ShowMapStatistics()` - Показать статистику
- `ClearMap()` - Очистить карту

**Жизненный цикл:** MonoBehaviour, живет в сцене

---

### 5. Analysis Layer (Слой анализа)

#### MapAnalyzer (Static Utility)
**Ответственность:** Анализ и валидация сгенерированной карты

**Метрики:**
- `ResourceDensity` - Процент ресурсных тайлов
- `CombatDensity` - Процент боевых тайлов
- `TileDistribution` - Распределение по типам
- `IsBalanced` - Базовые проверки корректности

**Проверки:**
- Наличие ровно 1 замка игрока
- Наличие хотя бы 1 вражеского замка
- Баланс ресурсов (30-70%)
- Разумное количество препятствий
- Разнообразие тайлов

**Рекомендации:**
- Недостаток определенных ресурсов
- Слишком много/мало событий
- Проблемы с доступностью

**Использование:**
```csharp
var analysis = MapAnalyzer.Analyze(generatedMap);
MapAnalyzer.PrintReport(analysis);
```

---

## 🔄 Поток данных

```
Config (Asset)
    ↓
Generator.GenerateMap() → Dictionary<Vector3Int, TileType>
    ↓
Analyzer.Analyze() → AnalysisResult
    ↓
Builder.BuildMap() → Physical Tiles in Scene
    ↓
(Optional) Builder.SaveToMapSettings() → MapSettings Asset
```

## 🎯 Design Patterns

### 1. **Builder Pattern**
`MapBuilder` строит сложный объект (карту) пошагово

### 2. **Strategy Pattern**
Разные конфигурации = разные стратегии генерации

### 3. **Factory Pattern**
`TileCreate()` методы в MapSettings

### 4. **Facade Pattern**
`MapGeneratorController` - простой интерфейс для сложной системы

### 5. **Static Utility Pattern**
`MapAnalyzer` - чистые функции без состояния

## 🔐 Принципы SOLID

### Single Responsibility Principle ✅
Каждый класс имеет одну ответственность:
- Config → Хранение параметров
- Generator → Генерация данных
- Builder → Построение физики
- Controller → Координация
- Analyzer → Анализ

### Open/Closed Principle ✅
Система открыта для расширения (наследование Generator), закрыта для модификации

### Liskov Substitution Principle ✅
Можно заменить Generator на наследника с кастомной логикой

### Interface Segregation Principle ✅
Классы не зависят от интерфейсов, которые не используют

### Dependency Inversion Principle ✅
Controller зависит от абстракций (ScriptableObject), не конкретных реализаций

## 🧪 Тестируемость

### Unit-тестируемые компоненты:
- ✅ `ProceduralMapGenerator` - чистая логика
- ✅ `MapAnalyzer` - статические методы
- ✅ `MapGenerationConfig.GetRandomTileType()` - детерминированно с seed

### Integration-тестируемые:
- ✅ `MapBuilder` - с mock префабами
- ✅ `MapGeneratorController` - в тестовой сцене

## 📊 Производительность

### Time Complexity
- **Grid Generation:** O(r²) где r = radius
- **Tile Placement:** O(n) где n = tiles
- **Connectivity Check:** O(n)
- **Building:** O(n)
- **Total:** O(n) = O(r²)

### Space Complexity
- **Generated Data:** O(n) - словарь позиций
- **Physical Map:** O(n) - GameObjects в сцене
- **Temporary:** O(n) - для BFS/pathfinding

### Оптимизации
- ✅ Генерация в editor-time (не runtime)
- ✅ Единожды созданные префабы
- ✅ Ленивая инициализация
- ✅ Эффективные структуры данных (Dictionary, HashSet)

## 🔮 Расширяемость

### Добавление нового типа тайла
```csharp
// 1. Добавить в enum
public enum TileType { ..., NewType }

// 2. Добавить процент в Config
public float NewTypePercentage = 5f;

// 3. Обновить GetRandomTileType()
cumulative += NewTypePercentage;
if (roll < cumulative) return TileType.NewType;

// 4. Зарегистрировать префаб в Controller
```

### Кастомный генератор
```csharp
public class BiomeMapGenerator : ProceduralMapGenerator
{
    protected override TileType DetermineTileType(Vector3Int pos, float dist)
    {
        // Ваша логика биомов
        int biome = CalculateBiome(pos);
        return GetTileForBiome(biome);
    }
}
```

### Добавление нового правила
```csharp
// В MapGenerationConfig
public bool MyCustomRule = true;

// В ProceduralMapGenerator.GenerateMap()
if (_config.MyCustomRule)
{
    ApplyCustomRule();
}
```

## 📝 Best Practices

1. **Всегда используйте seed** для тестирования
2. **Анализируйте карты** перед релизом
3. **Сохраняйте удачные конфиги** как assets
4. **Разделяйте генерацию и построение** для гибкости
5. **Используйте Editor tools** для итерации

## 🎓 Обучающие ресурсы

Система реализует концепции:
- **Procedural Content Generation (PCG)**
- **Hexagonal Grid Systems (Cube Coordinates)**
- **Graph Algorithms (BFS)**
- **Weighted Random Selection**
- **ScriptableObject Architecture**

---

**Автор:** Unity Expert Procedural Generation Specialist  
**Версия:** 1.0.0  
**Дата:** 2025-10-06

