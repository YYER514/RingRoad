using System;

public static class SelfExtensions
{
    /// <summary>
    /// 如果布尔值为 true，则执行指定的操作。
    /// </summary>
    /// <param name="condition">布尔条件</param>
    /// <param name="action">要执行的操作</param>
    public static void IfTrue(this bool condition, Action action)
    {
        if (condition)
        {
            action?.Invoke();
        }
    }
  public static  int CharToInt(char c)
    {
        // 检查是否为数字字符
        if (char.IsDigit(c))
        {
            return c - '0'; // ASCII 差值
        }
        else
        {
            return -1;
        }
    }

}
