export abstract class humanise {
  public static points(points: number) {
    const floor = Math.floor(points);
    if (floor == points) {
      return points.toString();
    }

    const decimal = points - floor;
    let decimalStr;
    if (decimal == 0.25)
      decimalStr = "¼";
    else if (decimal == 0.5)
      decimalStr = "½";
    else if (decimal == 0.75)
      decimalStr = "¾";
    else {
      // Substring rop the 0.
      const fixed = decimal.toFixed(2).substring(2);
      const suffix =
        fixed.endsWith("0")
        ? fixed.substring(0, 1)
        : fixed;
      decimalStr = "." + suffix;
    }

    if (floor == 0)
      return decimalStr;

    return `${floor}${decimalStr}`;
  }
}
