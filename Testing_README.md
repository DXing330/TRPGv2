# Unity Game Testing Suite for Older Unity Versions

This comprehensive testing suite provides thorough testing capabilities for all major game systems, specifically designed to work with older Unity versions (Unity 5.x to Unity 2019.x). The tests use the `[ContextMenu]` approach instead of the Unity Test Runner for maximum compatibility.

## 📋 Test Files Created

### Core System Tests
- **`BattleManagerTester.cs`** - Tests for the tactical battle system
- **`PartyDataManagerTester.cs`** - Tests for party management and data handling
- **`RequestBoardTester.cs`** - Tests for quest/request generation and management
- **`MoveCostManagerTester.cs`** - Tests for movement calculations and pathfinding
- **`ArmoryManagerTester.cs`** - Tests for equipment and inventory systems
- **`SaveLoadTester.cs`** - Tests for save/load functionality and data persistence

### Master Test Runner
- **`MasterTestRunner.cs`** - Centralized test execution and reporting system

## 🚀 How to Set Up the Tests

### 1. Add Test Files to Your Project
Place all test files in appropriate directories within your Unity project:
```
/Combat/Testers/BattleManagerTester.cs
/Combat/Testers/MoveCostManagerTester.cs
/GameManagement/PartyDataManagerTester.cs
/GameManagement/SaveLoadTester.cs
/Guild/RequestBoardTester.cs
/Equipment/ArmoryManagerTester.cs
/MasterTestRunner.cs (in root or a dedicated Testing folder)
```

### 2. Create Test GameObjects
1. Create empty GameObjects in your test scenes
2. Add the appropriate tester scripts as components
3. Assign the required references in the inspector

### 3. Set Up the Master Test Runner
1. Create a GameObject called "Master Test Runner"
2. Add the `MasterTestRunner` script
3. Assign all individual tester components to the Master Test Runner

## 🎮 How to Run Tests

### Individual Test Execution
1. Select the GameObject with the desired tester component
2. In the Inspector, right-click on the script header
3. Choose from the available Context Menu options:
   - "Run All [System] Tests" - Runs complete test suite for that system
   - Individual test methods for targeted testing

### Master Test Execution
1. Select the Master Test Runner GameObject
2. Right-click on the MasterTestRunner script in Inspector
3. Choose from these options:
   - **"Run All Tests"** - Executes all test suites sequentially
   - **"Run Battle System Tests"** - Tests battle-related systems only
   - **"Run Data Management Tests"** - Tests save/load and party data systems
   - **"Run UI System Tests"** - Tests UI and quest board systems
   - **"Generate Test Report"** - Creates a comprehensive test report
   - **"Quick System Check"** - Validates test setup and Unity compatibility

## 📊 Understanding Test Results

### Individual Test Results
Each test outputs results to the Unity Console with:
- ✓ **PASSED** tests in regular log entries
- ✗ **FAILED** tests in error log entries
- Detailed information about what was tested

### Master Test Reports
The Master Test Runner provides comprehensive reports including:
- Total test count and pass/fail ratios
- Execution time for each test suite
- Overall system health assessment
- Performance metrics

### Test Result Interpretation
- **90%+ Pass Rate**: 🎉 Excellent - System is robust
- **75-89% Pass Rate**: 👍 Good - Minor issues to address
- **50-74% Pass Rate**: ⚠️ Needs Work - Significant issues
- **<50% Pass Rate**: 🚨 Critical - Major problems require immediate attention

## 🔧 Test Configuration

### Required Component Assignments
Each tester requires specific component references:

**BattleManagerTester:**
- BattleManager battleManager
- BattleMap testMap
- MoveCostManager moveManager
- TacticActor testActor

**PartyDataManagerTester:**
- PartyDataManager partyDataManager
- StatDatabase testActorStats
- Equipment testEquipment

**RequestBoardTester:**
- RequestBoard requestBoard
- Request dummyRequest
- GuildCard testGuildCard

**MoveCostManagerTester:**
- MoveCostManager moveCostManager
- BattleMap testBattleMap
- TacticActor testActor

**ArmoryManagerTester:**
- ArmoryManager armoryManager
- Equipment testEquipment
- EquipmentInventory testInventory

**SaveLoadTester:**
- SavedDataManager savedDataManager
- List<SavedData> testSavedDataComponents

## ⚙️ Compatibility Features for Older Unity Versions

### Unity 5.x Compatibility
- No use of newer Unity APIs
- Compatible with older C# language features
- Uses `[ContextMenu]` instead of Unity Test Runner
- Standard MonoBehaviour approach

### Unity 2017-2019 Compatibility
- Uses string interpolation where supported
- Graceful fallback for older syntax
- Compatible with older .NET frameworks

### Cross-Platform Support
- Works on Windows, Mac, and Linux
- Compatible with standalone and mobile builds
- No platform-specific dependencies

## 🧪 Test Coverage

### Battle System (BattleManagerTester)
- ✅ State transitions and turn management
- ✅ Actor movement and pathfinding
- ✅ Attack system and skill activation
- ✅ Battle end conditions
- ✅ Mental state handling
- ✅ Tile interactions

### Party Management (PartyDataManagerTester)
- ✅ Party member addition/removal
- ✅ Equipment management
- ✅ Save/load functionality
- ✅ Temporary party members
- ✅ Party capacity and guild rank
- ✅ Rest and exhaustion systems

### Quest System (RequestBoardTester)
- ✅ Quest generation (Delivery, Defeat, Escort)
- ✅ Quest acceptance and completion
- ✅ Location generation
- ✅ Reward and penalty calculations
- ✅ UI integration

### Movement System (MoveCostManagerTester)
- ✅ Pathfinding algorithms
- ✅ Movement cost calculations
- ✅ Reachable tile detection
- ✅ Attack range calculations
- ✅ Movement validation

### Equipment System (ArmoryManagerTester)
- ✅ Equipment properties and stats
- ✅ Inventory management
- ✅ Equipment database operations
- ✅ Equip/unequip functionality
- ✅ UI integration

### Save/Load System (SaveLoadTester)
- ✅ Basic save/load operations
- ✅ Data persistence
- ✅ File handling
- ✅ Error handling and edge cases
- ✅ Multiple data source independence

## 🐛 Troubleshooting

### Common Issues
1. **"Component not assigned"** - Ensure all required references are set in Inspector
2. **"No test data available"** - Verify test setup objects exist in the scene
3. **"Method not found"** - Check that component interfaces match expected method signatures

### Performance Considerations
- Tests may take 30-60 seconds to complete fully
- Large data integrity tests may be slower on older hardware
- Consider running individual test suites for faster iteration

### Debugging Failed Tests
1. Check Unity Console for detailed error messages
2. Verify component references are properly assigned
3. Ensure test data objects are correctly configured
4. Use individual test methods to isolate issues

## 📈 Best Practices

### Regular Testing
- Run tests after major code changes
- Use Quick System Check before important builds
- Monitor pass rates over time

### Test Maintenance
- Update test data when adding new features
- Verify tests after Unity version upgrades
- Keep test components updated with system changes

### Performance Optimization
- Run specific test suites during development
- Use full test suite for release validation
- Consider test execution time in CI/CD pipelines

## 🔗 Integration with Development Workflow

### Pre-Commit Testing
1. Run Quick System Check
2. Execute relevant test suites for changed systems
3. Verify no new test failures

### Release Testing
1. Run complete test suite via Master Test Runner
2. Generate and review comprehensive test report
3. Address any critical issues before release

### Continuous Integration
While designed for manual execution, these tests can be integrated into automated systems by:
1. Creating test runner scripts that call the test methods
2. Parsing Unity Console output for results
3. Generating automated reports

---

**Note**: This testing suite is specifically designed for older Unity versions and uses proven, stable testing patterns. The `[ContextMenu]` approach ensures compatibility across Unity 5.x through Unity 2019.x while providing comprehensive test coverage for all major game systems.