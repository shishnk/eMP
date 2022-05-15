namespace eMP_3;

public static class Integration {
    public static double GaussSegment(Func<Point3D, double> psiI, Func<Point3D, double> psiJ, Point3D firstPoint, Point3D secondPoint) {
        var quadratures = Quadratures.SegmentGaussOrder9();

        double hx = firstPoint.X - secondPoint.X;
        double hy = firstPoint.Y - secondPoint.Y;
        double hz = firstPoint.Z - secondPoint.Z;
        double result = 0.0;

        foreach (var qi in quadratures) {
            foreach (var qj in quadratures) {
                foreach (var qk in quadratures) {
                    Point3D point = new(((qi.Node * hx) + firstPoint.X + secondPoint.X) / 2.0,
                                      ((qj.Node * hy) + firstPoint.Y + secondPoint.Y) / 2.0,
                                      ((qk.Node * hz) + firstPoint.Z + secondPoint.Z) / 2.0);

                    result += psiI(point) * psiJ(point) * qi.Weight * qj.Weight * qk.Weight;
                }
            }
        }

        return result * hx * hy * hz / 8.0;
    }
}