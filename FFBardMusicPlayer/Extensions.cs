using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

public static class Extensions {

    public static string Repeat(this string s, int n)
        => new StringBuilder(s.Length * n).Insert(0, s, n).ToString();

	public static string ToQuery(this Dictionary<string, string> dict) {
		string query = string.Empty;
		var validParameters = dict.Where(p => !String.IsNullOrEmpty(p.Value));
		var formattedParameters = validParameters.Select(p => p.Key + "=" + Uri.EscapeDataString(p.Value));
		query += string.Join("&", formattedParameters.ToArray());
		return query;
	}
	public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T> {
		if(val == null)
			throw new ArgumentNullException(nameof(val), "is null.");
		if(min == null)
			throw new ArgumentNullException(nameof(min), "is null.");
		if(max == null)
			throw new ArgumentNullException(nameof(max), "is null.");
		//If min <= max, clamp
		if(min.CompareTo(max) <= 0)
			return val.CompareTo(min) < 0 ? min : val.CompareTo(max) > 0 ? max : val;
		//If min > max, clamp on swapped min and max
		return val.CompareTo(max) < 0 ? max : val.CompareTo(min) > 0 ? min : val;
	}
}

public static class FormExtensions {
	public static void Invoke<TControlType>(this TControlType control, Action<TControlType> del)
		where TControlType : Control {
		if(control.InvokeRequired)
			control.Invoke(new Action(() => del(control)));
		else
			del(control);
	}
	public static double GetDistance(this PointF p1, PointF p2) {
		return Math.Sqrt(Math.Pow((p2.X - p1.X), 2) + Math.Pow((p2.Y - p1.Y), 2));
	}
	public static void DrawCircle(this Graphics g, Pen pen,
								  float centerX, float centerY, float radius) {
		g.DrawEllipse(pen, centerX - radius, centerY - radius,
					  radius + radius, radius + radius);
	}

	public static void FillCircle(this Graphics g, Brush brush,
								  float centerX, float centerY, float radius) {
		g.FillEllipse(brush, centerX - radius, centerY - radius,
					  radius + radius, radius + radius);
	}
	public static Point FormRelativeLocation(this Control control, Form form = null) {
		if(form == null) {
			form = control.FindForm();
			if(form == null) {
				throw new Exception("Form not found.");
			}
		}

		Point cScreen = control.PointToScreen(control.Location);
		Point fScreen = form.Location;
		Point cFormRel = new Point(cScreen.X - fScreen.X, cScreen.Y - fScreen.Y);

		return cFormRel;

	}
}