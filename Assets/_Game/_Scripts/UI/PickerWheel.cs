using UnityEngine ;
using UnityEngine.UI ;
using TMPro;
using DG.Tweening ;
using UnityEngine.Events ;
using System.Collections.Generic ;

namespace EasyUI.PickerWheelUI {

   public class PickerWheel : MonoBehaviour {

      [Header ("References :")]
      [SerializeField] private GameObject linePrefab ;
      [SerializeField] private Transform linesParent ;

      [Space]
      [SerializeField] private Transform PickerWheelTransform ;
      [SerializeField] private Transform wheelCircle ;
      [SerializeField] private GameObject wheelPiecePrefab ;
      [SerializeField] private List<GameObject> list_wheelPiece;
      [SerializeField] private Transform wheelPiecesParent ;

      [Space]
      [Header ("Sounds :")]
      //[SerializeField] private AudioSource audioSource ;
      //[SerializeField] private AudioClip tickAudioClip ;
      //[SerializeField] [Range (0f, 1f)] private float volume = .5f ;
      //[SerializeField] [Range (-3f, 3f)] private float pitch = 1f ;

      [Space]
      [Header ("Picker wheel settings :")]
      [Range (1, 20)] public int spinDuration = 8 ;
      [SerializeField] [Range (.2f, 2f)] private float wheelSize = 1f ;

      [Space]
      [Header ("Picker wheel pieces :")]
      public WheelPiece[] wheelPieces ;

      // Events
      private UnityAction onSpinStartEvent ;
        private UnityAction<WheelPiece> onSpinEndEvent ;


        private bool _isSpinning = false ;

      public bool IsSpinning { get { return _isSpinning ; } }


      private Vector2 pieceMinSize = new Vector2 (81f, 146f) ;
      private Vector2 pieceMaxSize = new Vector2 (144f, 213f) ;
      private int piecesMin = 2 ;
      private int piecesMax = 12 ;

      private float pieceAngle ;
      private float halfPieceAngle ;
      private float halfPieceAngleWithPaddings ;


      private double accumulatedWeight ;
      private System.Random rand = new System.Random () ;

      private List<int> nonZeroChancesIndices = new List<int> () ;

      public void StartGenerate () {
            accumulatedWeight =0;
            wheelCircle.localEulerAngles = Vector3.zero;
            Debug.Log("chạy vào đây");
         pieceAngle = 360 / wheelPieces.Length ;
         halfPieceAngle = pieceAngle / 2f ;
         halfPieceAngleWithPaddings = halfPieceAngle - (halfPieceAngle / 4f) ;

         Generate() ;  

         CalculateWeightsAndIndices () ;
         if (nonZeroChancesIndices.Count == 0)
            Debug.LogError ("You can't set all pieces chance to zero") ;


         //SetupAudio () ;

      }

      //private void SetupAudio () {
      //   audioSource.clip = tickAudioClip ;
      //   audioSource.volume = volume ;
      //   audioSource.pitch = pitch ;
      //}

      private void Generate () {
            if(list_wheelPiece.Count > 0) {
                for (int i = 0; i < list_wheelPiece.Count; i++) {
                    Destroy(list_wheelPiece[i]);
                }
                list_wheelPiece.Clear();
            }
         for (int i = 0; i < wheelPieces.Length; i++)
            DrawPiece (i) ;

            wheelCircle.transform.localEulerAngles = new Vector3(0, 0, 0);
      }

      private void DrawPiece (int index) {
         WheelPiece piece = wheelPieces [ index ] ;
         Transform pieceTrns = InstantiatePiece (index).transform.GetChild (0);
         pieceTrns.GetChild (2).GetComponent <TextMeshProUGUI> ().text = piece.Amount.ToString () ;
            // if (piece.IsCup)
            // {
            //     pieceTrns.GetChild(0).GetComponent<Image>().rectTransform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            //     pieceTrns.GetChild(0).GetComponent<Image>().sprite = piece.Icon;
            //     pieceTrns.GetComponent<VerticalLayoutGroup>().spacing = -130f;
            // }
            // else if (piece.isSkin)
            // {
            //     pieceTrns.GetChild (0).GetComponent <Image> ().sprite = piece.Icon ;
            //     pieceTrns.GetChild(0).GetComponent<Image>().rectTransform.localScale = new Vector3(1f, 1f, 1f);
            //     pieceTrns.GetChild(2).transform.gameObject.SetActive(false);
            // }
            // else
            // {
            pieceTrns.GetChild(0).GetComponent<Image>().sprite = piece.Icon;   // thay ảnh vào chỗ này
            // }
         //pieceTrns.GetChild (1).GetComponent <Text> ().text = piece.Label ;

        //Line
        Transform lineTrns = Instantiate (linePrefab, linesParent.position, Quaternion.identity, linesParent).transform ;
         lineTrns.RotateAround (wheelPiecesParent.position, Vector3.back, (pieceAngle * index) + halfPieceAngle) ;

         pieceTrns.RotateAround (wheelPiecesParent.position, Vector3.back, pieceAngle * index) ;
         RectTransform pieceTrans = pieceTrns.parent.GetComponent<RectTransform>();
         pieceTrans.localEulerAngles = new Vector3(0f, 0, pieceTrans.localEulerAngles.z); 
      }

      private GameObject InstantiatePiece (int index) {
            list_wheelPiece.Add(Instantiate(wheelPiecePrefab, wheelPiecesParent.position, Quaternion.identity, wheelPiecesParent));
            return list_wheelPiece [index];
        }


      public void Spin() {
            if (!_isSpinning) {
            _isSpinning = true ;
               
                if (onSpinStartEvent != null)
               onSpinStartEvent.Invoke () ;

            int index = GetRandomPieceIndex () ;
                Debug.LogWarning("Index Wheel:" + index);
            WheelPiece piece = wheelPieces [ index ] ;

            if (piece.Chance == 0 && nonZeroChancesIndices.Count != 0) {
               index = nonZeroChancesIndices [ Random.Range (0, nonZeroChancesIndices.Count) ] ;
               piece = wheelPieces [ index ] ;
            }

            float angle = -(pieceAngle * index); //-(pieceAngle * index)

            //     float rightOffset = (angle - halfPieceAngleWithPaddings) % 360 ;
            // float leftOffset = (angle + halfPieceAngleWithPaddings) % 360 ;

            // float randomAngle = angle;
            // Debug.Log("góc lệch: " + randomAngle);

            //  randomAngle = 45f * times;
            // int times = (int) randomAngle % 45;
            // float gocLech = randomAngle % 45;

            Vector3 targetRotation = Vector3.back * (angle + 2*360 * spinDuration);
               //  targetRotation.y = 180f;

            //float prevAngle = wheelCircle.eulerAngles.z + halfPieceAngle ;
            float prevAngle, currentAngle ;
            prevAngle = currentAngle = wheelCircle.eulerAngles.z ;

            bool isIndicatorOnTheLine = false ;

            wheelCircle.DORotate (targetRotation, spinDuration, RotateMode.Fast)
            .SetEase (Ease.InOutQuart)
            .OnUpdate (() => {
               float diff = Mathf.Abs (prevAngle - currentAngle) ;
               if (diff >= halfPieceAngle) {
                  if (isIndicatorOnTheLine) {
                     //audioSource.PlayOneShot (audioSource.clip) ;
                  }
                  prevAngle = currentAngle ;
                  isIndicatorOnTheLine = !isIndicatorOnTheLine;

               }
               wheelCircle.localEulerAngles = new Vector3(0, 0, wheelCircle.localEulerAngles.z);
               // float normalized = NormalizedValue(piece);
               // Debug.Log("normalized " + normalized);
               currentAngle = wheelCircle.eulerAngles.z; //+ normalized
            })
            .OnComplete (() => {
               _isSpinning = false ;
               if (onSpinEndEvent != null)
                  onSpinEndEvent.Invoke (piece) ;

               onSpinStartEvent = null ;
               onSpinEndEvent = null ;
            }) ;

         }
      }

      public void OnSpinStart (UnityAction action) {
         onSpinStartEvent = action ;
      }

        public void OnSpinEnd(UnityAction<WheelPiece> action)
        {
            onSpinEndEvent = action;
        }

        private int GetRandomPieceIndex () {
         double r = rand.NextDouble () * accumulatedWeight ;

         for (int i = 0; i < wheelPieces.Length; i++)
            if (wheelPieces [ i ]._weight >= r)
               return i ;

         return 0 ;
      }

      private void CalculateWeightsAndIndices () {
         for (int i = 0; i < wheelPieces.Length; i++) {
            WheelPiece piece = wheelPieces [ i ] ;

            //add weights:
            accumulatedWeight += piece.Chance ;
            piece._weight = accumulatedWeight ;

            //add index :
            piece.Index = i ;

            //save non zero chance indices:
            if (piece.Chance > 0)
               nonZeroChancesIndices.Add (i) ;
         }
      }

      private void OnValidate () {
         if (PickerWheelTransform != null)
            PickerWheelTransform.localScale = new Vector3 (wheelSize, wheelSize, 1f) ;

         if (wheelPieces.Length > piecesMax || wheelPieces.Length < piecesMin)
            Debug.LogError ("[ PickerWheelwheel ]  pieces length must be between " + piecesMin + " and " + piecesMax) ;
      }

        private float NormalizedValue(WheelPiece wheelPiece)
		{
            int childOrder = 0;
            for (int i = 0; i < wheelPieces.Length; i++)
			{
                if (wheelPiece.Equals(wheelPieces[i]))
				{
                    childOrder = i;
                    break;
				}
			}

            if (childOrder == 1 || childOrder == 4) return 120f;
            else if (childOrder == 2 || childOrder == 5) return -120f;
            else return 0;
		}

        private float NormalizedIndex(int index)
        {
            if (index == 4) return 1;
            else if (index == 1) return 4;
            else if (index == 5) return 3;
            else if (index == 3) return 5;
            else return index;
        }

    }
}