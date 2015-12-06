using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace LighTake.Infrastructure.Web.Route
{
  public class ModelParamterBinderAttribute : CustomModelBinderAttribute
  {
    public override IModelBinder GetBinder()
    {
      return new ModelParameterBinder();
    }

    private class ModelParameterBinder : IModelBinder
    {
      public object BindModel( ControllerContext controllerContext, ModelBindingContext bindingContext )
      {

        var model = controllerContext.RouteData.Values["model"];

        if ( model == null )
          return null;

        if ( bindingContext.ModelType.IsAssignableFrom( model.GetType() ) )
          return model;

        return null;

      }
    }


  }
}
