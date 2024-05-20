# Compass
Compass is a pathfinding library and one of the "pathfinding trio" (the other two being the baking tool Toaster and the intersection detection library Knot). <br/>
**Compass 是一个寻路库，是"寻路三件套"之一（另外两个是：烘焙工具 Toaster、交叉检测库 Knot）。**

# Readiness
It is one of my early works, now seen as full of flaws, and I plan to rewrite it when I have the time.<br/>
**它是我的早期作品，现在来看充满瑕疵，有空会考虑对它重写。**

# Feature
## Implemented
* A non-intrusive pathfinding solution that doesn't hijack the upper-level Transform (just like its name: a compass only shows the way, it doesn't drive).<br/>
  **一套不劫持上层 Transform 的寻路方案（正如其名：指南针只指路，它不负责开车）;**
* Supporting various heuristic function cost calculations.<br/>
  **支撑多种启发式函数计算成本；**
* Corner trapping prevention.<br/>
  **拐角防卡；**
* Volume trapping prevention.<br/>
  **体积防卡。**

## Unimplemented
* Path simplification<br/>
  **路径简化；**
* Path curve interpolation.<br/>
  **路径曲线插值。**
