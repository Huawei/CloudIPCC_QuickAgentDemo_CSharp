using System;
using System.Windows.Media.Animation;
using System.Windows;

namespace QuickAgent.Src.Common
{
    public class AnimationList
    {
        private static AnimationList animationList;

        public Boolean IsExp { get; set; }
        //单例模式
        public static AnimationList Instance()
        {
            if (animationList == null)
            {
                animationList = new AnimationList();
            }
            return animationList;
        }

        bool isOver = false;
        //DoubleAnimation exhibitProperty =new DoubleAnimation();
        /// <summary>
        /// 主窗口贴标隐藏
        /// </summary>
        public void DisplayAndHidden(Window window, Double fromValue, Double toValue)
        {
            if (isOver)
            {
                return;
            }

            DoubleAnimation exhibitProperty =
                  new DoubleAnimation(fromValue, toValue, new Duration(TimeSpan.FromSeconds(0.2)), FillBehavior.Stop);
            exhibitProperty.Completed += (s, e) =>
            {
                isOver = false;
                if (IsExp)
                {
                    window.Top = 0;
                    IsExp = false;
                }
                else
                {
                    window.Top = -window.ActualHeight + 1;
                    IsExp = true;
                }
            };
            window.BeginAnimation(Window.TopProperty, exhibitProperty);
            isOver = true;
        }
    }
}
