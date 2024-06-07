
namespace Managers
{

 public static class ExtensionsManager
 {
  #region Public Methods
  public static float Map(this float value, float fromMin, float fromMax, float toMin, float toMax)
  {
   return (value - fromMin) / (fromMax - fromMin) * (toMax - toMin) + toMin;
  }
  #endregion
 }
}
