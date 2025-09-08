using System;

public static class SelfExtensions
{
    /// <summary>
    /// �������ֵΪ true����ִ��ָ���Ĳ�����
    /// </summary>
    /// <param name="condition">��������</param>
    /// <param name="action">Ҫִ�еĲ���</param>
    public static void IfTrue(this bool condition, Action action)
    {
        if (condition)
        {
            action?.Invoke();
        }
    }
  public static  int CharToInt(char c)
    {
        // ����Ƿ�Ϊ�����ַ�
        if (char.IsDigit(c))
        {
            return c - '0'; // ASCII ��ֵ
        }
        else
        {
            return -1;
        }
    }

}
