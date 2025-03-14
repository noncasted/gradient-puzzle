namespace Services
{
    public interface IMaskRenderOptions
    {
        RenderMaskData Get(int index);
        RenderMaskData GetFromBack(int index);
    }
}