namespace CurvedUIUtility
{
    public interface ICurveable
    {
        bool HasCurvedThisFrame { get; set; }

        void CheckPosition();

        void UpdateCurvature();

        void UpdateMatrices();
    }
}
