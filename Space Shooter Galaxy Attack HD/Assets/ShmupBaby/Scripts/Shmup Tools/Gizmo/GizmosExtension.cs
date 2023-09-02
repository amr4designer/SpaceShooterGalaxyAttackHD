using UnityEngine;

namespace ShmupBaby {

	/// <summary>
	/// Contains a method to draw basic shapes using a gizmo
	/// </summary>
	public static class GizmosExtension  {

		#region Rect

		/// <summary>
		/// Draws the given rectangle on the XY plane at Z = 0.
		/// </summary>
		/// <param name="rect">Rect to draw.</param>
		public static void DrawRect ( Rect rect ) {
			DrawRect (rect, 0);
		}

		/// <summary>
		/// Draws the given rectangle on the XY plane at Z = depth.
		/// </summary>
		/// <param name="rect">Rectangle to draw.</param>
		/// <param name="depth">Rectangle position on the Z Axis.</param>
		public static void DrawRect ( Rect rect , float depth ) {
			Gizmos.DrawLine (new Vector3 (rect.xMax, rect.yMax, depth), new Vector3 (rect.xMin, rect.yMax, depth));
			Gizmos.DrawLine (new Vector3 (rect.xMin, rect.yMax, depth), new Vector3 (rect.xMin, rect.yMin, depth));
			Gizmos.DrawLine (new Vector3 (rect.xMin, rect.yMin, depth), new Vector3 (rect.xMax, rect.yMin, depth));
			Gizmos.DrawLine (new Vector3 (rect.xMax, rect.yMin, depth), new Vector3 (rect.xMax, rect.yMax, depth));
		}

		#endregion

		#region Circle 

		/// <summary>
		/// Draws the a circle with a given radius in the XY plane at Z = 0.
		/// </summary>
		/// <param name="center">Center of the circle in world space.</param>
		/// <param name="radius">Radius of the circle in world units.</param>
		public static void DrawCircle ( Vector2 center , float radius ){
			DrawCircle (center, radius , 0 , 3f );
		}

		/// <summary>
		/// Draws the a circle with a given radius in the XY plane at Z = depth.
		/// </summary>
		/// <param name="center">Center of the circle in world space.</param>
		/// <param name="radius">Radius of the circle in world units.</param>
		/// <param name="depth">Circle Position on the Z Axis.</param>
		public static void DrawCircle ( Vector2 center , float radius , float depth  ){
			DrawCircle (center, radius, depth, 3f);
		}

		/// <summary>
		/// Draws the a circle with a given radius in the XY plane at Z = depth.
		/// </summary>
		/// <param name="center">Center of the circle in world space.</param>
		/// <param name="radius">Radius of the circle in world unit.</param>
		/// <param name="depth">Circle Position on the Z Axis.</param>
		/// <param name="deltaAngle">Circle gets more accurate the lower the deltaAngle is.</param>
		public static void DrawCircle ( Vector2 center , float radius , float depth , float deltaAngle ){

			int SegmentNum = Mathf.CeilToInt (360f/deltaAngle);

			float RadialDegree = deltaAngle*Mathf.Deg2Rad ; 

			Vector3 Center = new Vector3 (center.x, center.y, 0 );

			for (int i = 0; i < SegmentNum; i++) {
				Vector3 From;
				Vector3 To;
				if (i == SegmentNum - 1) {
					From = new Vector3 (
						Mathf.Cos (RadialDegree*i)*radius ,
						Mathf.Sin (RadialDegree*i)*radius ,
						depth )+Center;
					To = new Vector3 (
						radius,
						0,
						depth )+Center;
				} else {
					From = new Vector3 (
						Mathf.Cos (RadialDegree*i)*radius ,
						Mathf.Sin (RadialDegree*i)*radius ,
						depth )+Center;
					To = new Vector3 (
						Mathf.Cos (RadialDegree*(i+1))*radius,
						Mathf.Sin (RadialDegree*(i+1))*radius,
						depth )+Center;
				}
				Gizmos.DrawLine (From, To);
			}
		}

		#endregion

	}

}

