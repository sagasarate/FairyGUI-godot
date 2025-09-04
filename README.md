[Godot](https://docs.godotengine.org/)的[FairyGUI](https://www.fairygui.com/)c#运行库，在Unity版本的基础上结合Laya版本的运行库修改而成
由于Godot引擎本身功能的限制，不少特性无法支持：
不支持3DUI
不支持变灰
不支持滤镜
BlendMode有限制
不支持遮罩
不支持FGUI编辑器内制作的BmpFont，只能在Godot引擎内制作并注册进FGUI
文本和输入组件是使用Godot的对应组件包装而成，所以功能受限，比如除了RichTextField都不支持UBB，RichTextField也无法引用UI包中的图片，等等

Demo是用Laya版本的Demo改的，基于上述不支持的特性，某些Demo是无法正常工作的
