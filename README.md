# 简易 Gameplay 框架

这个项目是一个简易的游戏开发框架，包含了资源管理、场景切换和对象池等基础模块。框架设计简洁，适合用于快速开发游戏的核心功能。下面是各个模块的简要介绍以及使用说明。

## 目录

- [简易 Gameplay 框架](#简易-gameplay-框架)
  - [目录](#目录)
  - [模块概述](#模块概述)
  - [模块详情](#模块详情)
    - [UI 管理模块 (UIManager)](#ui-管理模块-uimanager)
      - [核心方法](#核心方法)
      - [层级结构](#层级结构)
      - [初始化说明](#初始化说明)
    - [UI交互动画 (MenuEventSystemHandler)](#ui交互动画-menueventsystemhandler)
      - [核心功能](#核心功能)
      - [核心方法](#核心方法-1)
      - [使用说明](#使用说明)
    - [事件中心模块 (EventCenter)](#事件中心模块-eventcenter)
      - [核心功能](#核心功能-1)
    - [场景管理模块 (ScenesMgr)](#场景管理模块-scenesmgr)
      - [核心方法](#核心方法-2)
    - [资源管理模块 (ResMgr)](#资源管理模块-resmgr)
      - [核心方法](#核心方法-3)
    - [对象池管理模块 (PoolMgr)](#对象池管理模块-poolmgr)
      - [核心方法](#核心方法-4)
  - [许可协议](#许可协议)
  
&nbsp;

## 模块概述

该框架提供了以下几个核心模块：
1. **UI管理模块**：管理所有的UI面板逻辑。
2. **UI交互动画**：基于DoTween实现的可交互UI的动画
3. **事件中心**：管理游戏内消息通信的中心。
4. **场景管理模块**：同步与异步场景加载功能。
5. **资源管理模块**：提供同步和异步资源加载的功能，支持对象的实例化。
6. **对象池管理模块**：实现了对象池（类似于缓存池），帮助有效管理和复用游戏对象，减少内存开销。

&nbsp;

## 模块详情

### UI 管理模块 (UIManager)
UIManager 模块负责游戏内所有 UI 面板的加载、显示、隐藏和事件监听。支持多层级 UI 管理，通过字典缓存已加载面板，避免重复加载，同时封装了 EventSystem 的基础事件注册逻辑。

#### 核心方法
- `ShowPanel<T>(string panelName, E_UI_Layer layer = E_UI_Layer.Mid, UnityAction<T> callBack = null)`:显示指定名称的 UI 面板，自动加载并挂载到对应层级，支持异步加载完成后的回调处理。
- `HidePanel(string panelName)`:隐藏并销毁指定名称的 UI 面板，并从缓存中移除。
- `GetPanel<T>(string name)`:获取一个已经显示的 UI 面板脚本，便于外部调用其接口。
- `AddCustomEventListener(UIBehaviour control, EventTriggerType type, UnityAction<BaseEventData> callBack)`:为任意 UI 控件添加事件监听（如点击、进入、离开等），封装了 EventTrigger 使用流程。

#### 层级结构
UI 面板将被自动添加到以下四个 `Canvas` 子节点之一：
- `Bot`：底层 UI（例如背景）
- `Mid`：中层 UI（如主界面）
- `Top`：上层 UI（如提示框）
- `System`：系统层 UI（如加载遮罩、系统弹窗）

#### 初始化说明
- 框架启动时，会自动加载 `UI/Canvas` 和 `UI/EventSystem` 预制体，并挂载至全局，不随场景销毁；
- 所有 UI 面板预制体需放置于 `Resources/UI/` 路径下，并命名与调用时一致；
- UI 控件事件注册统一通过 `AddCustomEventListener` 方法进行封装调用。

&nbsp;

### UI交互动画 (MenuEventSystemHandler)
`MenuEventSystemHandler` 模块用于处理菜单中 `UI` 元素的导航行为和交互动画效果。支持手柄/键盘方向输入与鼠标悬停时的自动选中控制，配合 `DoTween` 实现 `UI` 缩放反馈动画。

#### 核心功能
- 自动记录所有 `Selectable`，并在启用时设置默认选中项；
- 支持使用新输入系统 `InputSystem` 的导航绑定，手柄/键盘移动后自动恢复选中项；
- 鼠标悬停时自动设置选中项，实现鼠标与手柄输入共存；
- 为 `Select` 和 `Deselect` 添加放大/还原缩放动画，提升 `UI` 反馈感；
- 使用 `EventTrigger` 动态绑定交互事件，无需手动设置。

#### 核心方法
- `AddSelectionListener(Selectable)`：为 `UI` 控件绑定四种交互事件：选中、取消选中、鼠标进入、鼠标离开。
- `OnSelect(BaseEventData)`：选中时播放缩放放大动画。
- `OnDeselect(BaseEventData)`：取消选中时播放还原动画。
- `OnPointerEnter(BaseEventData)`：鼠标进入时设置为当前选中项。
- `OnPointerExit(BaseEventData)`：鼠标移出时清空选中状态。
- `OnNavigate(InputAction.CallbackContext)`：手柄/键盘导航事件回调，若 UI 无选中对象，恢复上次选中的项。

#### 使用说明
1. 在场景中挂载该组件；
2. 设置 Selectables 列表，指定要处理动画的 UI 元素；
3. 设置 FirstSelected，确保菜单激活时自动聚焦；
4. 配置 InputActionReference 绑定导航输入；

所有缩放动画通过 DoTween 实现，支持自定义缩放比例和过渡时长。

&nbsp;

### 事件中心模块 (EventCenter)
`EventCenter` 模块负责游戏内的事件管理，实现了观察者模式，支持带参数或不带参数的事件监听、触发与移除。该模块通过 `Dictionary` 存储事件名与对应的回调方法，支持泛型事件处理，并通过 `UnityAction` 实现事件回调的绑定与触发。

#### 核心功能
添加事件监听（支持泛型）
- `AddEventListener<T>(string name, UnityAction<T> action)`：添加一个带参数的事件监听。
- `AddEventListener(string name, UnityAction action)`：添加一个无参数的事件监听。

移除事件监听
- `RemoveEventListener<T>(string name, UnityAction<T> action)`：移除一个带参数的事件监听。
- `RemoveEventListener(string name, UnityAction action)`：移除一个无参数的事件监听。

事件触发
- `EventTrigger<T>(string name, T info)`：触发一个带参数的事件。
- `EventTrigger(string name)`：触发一个无参数的事件。

该模块为各个系统之间提供了解耦的通信方式，是整体架构中非常关键的工具模块。

&nbsp;

### 场景管理模块 (ScenesMgr)

`ScenesMgr` 模块负责场景的切换。提供了同步加载和异步加载场景的功能，并支持进度条更新事件。

#### 核心方法
- `LoadScene(string name, UnityAction fun)`：同步加载场景并执行加载完成后的回调。
- `LoadSceneAsyn(string name, UnityAction fun)`：异步加载场景并通过协程处理加载进度。
  
场景加载时通过事件中心 (`EventCenter`) 分发进度信息，便于外部UI更新。

&nbsp;


### 资源管理模块 (ResMgr)

`ResMgr` 模块负责游戏资源的加载，支持同步和异步加载。通过泛型支持多种资源类型的加载，并可自动实例化 `GameObject`。

#### 核心方法
- `Load<T>(string name)`：同步加载资源并返回。
- `LoadAsync<T>(string name, UnityAction<T> callback)`：异步加载资源并通过回调返回加载的资源。

&nbsp;

### 对象池管理模块 (PoolMgr)

`PoolMgr` 模块实现了对象池的功能，用于缓存和复用不常使用的对象（例如 `GameObject`）。对象池使用字典存储每个类型的对象池，能显著提高性能，避免频繁创建和销毁对象。

#### 核心方法
- `GetObj(string name, UnityAction<GameObject> callBack)`：从池中获取对象。如果池中没有对象，自动异步加载并实例化。
- `PushObj(string name, GameObject obj)`：将不再使用的对象放回池中。
- `Clear()`：清空所有对象池（通常在场景切换时使用）。
  
&nbsp;

## 许可协议

此项目采用 [MIT License](LICENSE)。

