namespace MortiseFrame.Compass {

    public enum HeuristicType {

        Manhattan,          // 曼哈顿距离, 也叫城市街区距离, 两点在各自坐标轴上的距离之和
        Euclidean,          // 欧几里得距离, 两点的直线距离
        Chebyshev,          // 切比雪夫距离, 两点在各自坐标轴上的距离的最大值
        Octile              // 八方向距离, 两点在各自坐标轴上的距离的最大值 

    }

}