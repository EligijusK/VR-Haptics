using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Es.InkPainter.Sample
{
	public class MousePainter : MonoBehaviour
	{

		public static MousePainter painter;
		/// <summary>
		/// Types of methods used to paint.
		/// </summary>
		[System.Serializable]
		private enum UseMethodType
		{
			RaycastHitInfo,
			WorldPoint,
			NearestSurfacePoint,
			DirectUV,
		}
		
		[SerializeField]
		RaycastPainting raycastPainting;
		
		[SerializeField]
		private Brush brush;

		[SerializeField]
		private UseMethodType useMethodType = UseMethodType.RaycastHitInfo;

		[SerializeField]
		bool erase = false;
		
		[SerializeField]
		float availableDistance = 0.1f;
		
		[SerializeField]
		float minBrushSize = 0.002f;
		
		float paintedDistance = 0.0f;
		Vector3 lastPaintedPosition = Vector3.zero;
		float originalBrushSize = 0.0f;

		private void Awake()
		{
			if (painter == null)
			{
				painter = this;
			}
		}

		private void Start()
		{
			originalBrushSize = brush.Scale;
		}

		private void Update()
		{
			// if(Input.GetMouseButton(0))
			// {
				// var ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
				bool success = true;
				if(raycastPainting != null && raycastPainting.GetWasHit())
				{
					RaycastHit hitInfo = raycastPainting.GetHit();
					var paintObject = hitInfo.transform.GetComponent<InkCanvas>();
					if (paintObject != null)
					{
						if (lastPaintedPosition != Vector3.zero)
						{
							paintedDistance += Vector3.Distance(lastPaintedPosition, hitInfo.point);
							brush.Scale = (1 - (paintedDistance / availableDistance)) * originalBrushSize;
							if (paintedDistance > availableDistance || brush.Scale < minBrushSize)
							{
								paintedDistance = availableDistance;
								brush.Scale = 0.0f;
								return;
							}
						}
						else
						{
							lastPaintedPosition = hitInfo.point;
						}


						if (paintedDistance < availableDistance)
						{
							switch (useMethodType)
							{
								case UseMethodType.RaycastHitInfo:
									success = erase
										? paintObject.Erase(brush, hitInfo)
										: paintObject.Paint(brush, hitInfo);
									break;

								case UseMethodType.WorldPoint:
									success = erase
										? paintObject.Erase(brush, hitInfo.point)
										: paintObject.Paint(brush, hitInfo.point);
									break;

								case UseMethodType.NearestSurfacePoint:
									success = erase
										? paintObject.EraseNearestTriangleSurface(brush, hitInfo.point)
										: paintObject.PaintNearestTriangleSurface(brush, hitInfo.point);
									break;

								case UseMethodType.DirectUV:
									if (!(hitInfo.collider is MeshCollider))
										Debug.LogWarning("Raycast may be unexpected if you do not use MeshCollider.");
									success = erase
										? paintObject.EraseUVDirect(brush, hitInfo.textureCoord)
										: paintObject.PaintUVDirect(brush, hitInfo.textureCoord);
									break;
							}
						}

						lastPaintedPosition = hitInfo.point;
					}

					if(!success)
						Debug.LogError("Failed to paint.");
				}
			// }
		}
		
		public void ResetPainting()
		{
			paintedDistance = 0.0f;
			lastPaintedPosition = Vector3.zero;
			brush.Scale = originalBrushSize;
		}
		
		public void SetAvailableDistance(float distance)
		{
			availableDistance = distance;
		}
		
		public void SetRaycastPainting(RaycastPainting raycastPainting)
		{
			this.raycastPainting = raycastPainting;
		}
		
		public bool CheckIfCanPaint()
		{
			return paintedDistance < availableDistance;
		}

		public void OnGUI()
		{
			if(GUILayout.Button("Reset"))
			{
				foreach(var canvas in FindObjectsOfType<InkCanvas>())
					canvas.ResetPaint();
			}
		}
	}
}