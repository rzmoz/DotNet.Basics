namespace DotNet.Basics.Autofac
{
    public static class AutofacExtensions
    {
        public static void Register(this AutofacBuilder builder, params IAutofacRegistrations[] registrations)
        {
            foreach (IAutofacRegistrations t in registrations)
                t.RegisterIn(builder);
        }
    }
}
