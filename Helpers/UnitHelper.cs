using warehouseapp.Enums;

namespace warehouseapp.Helpers
{
    public static class UnitHelper
    {
        public static string GetSymbol(UnitOfMeasurementEnum unit) => unit switch
        {
            UnitOfMeasurementEnum.Piece => "pcs",
            UnitOfMeasurementEnum.Kilogram => "kg",
            UnitOfMeasurementEnum.Gram => "g",
            UnitOfMeasurementEnum.Liter => "l",
            UnitOfMeasurementEnum.Milliliter => "ml",
            UnitOfMeasurementEnum.Box => "box",
            UnitOfMeasurementEnum.Pack => "pack",
            _ => ""
        };
    }
}
