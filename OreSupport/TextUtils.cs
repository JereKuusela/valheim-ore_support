using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace OreSupport;
public class Format {
  public const string FORMAT = "0.##";
  public static string String(string value, string color = "yellow") => "<color=" + color + ">" + value + "</color>";
  public static string Int(double value, string color = "yellow") => String(value.ToString("N0"), color);
  public static string JoinRow(IEnumerable<string> lines) => string.Join(", ", lines.Where(line => line != ""));
  public static string Coordinates(Vector3 coordinates, string format = "F0", string color = "yellow") {
    var values = coordinates.ToString(format).Replace("(", "").Replace(")", "").Split(',').Select(value => String(value.Trim(), color));
    return JoinRow(values);
  }
}
