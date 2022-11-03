using UnityEngine ;

namespace EasyUI.PickerWheelUI {
   [System.Serializable]
   public class WheelPiece {
      public UnityEngine.Sprite Icon ;
      //public string Label ;

      [Tooltip ("Reward amount")] public int Amount ;

      [Tooltip ("Probability in %")] 
      [Range (0f, 100f)] 
      public float Chance = 100f ;
      public ItemSpin_Type itemType;
      public int id;
      [HideInInspector] public int Index ;
      [HideInInspector] public double _weight = 0f ;
   }
}
