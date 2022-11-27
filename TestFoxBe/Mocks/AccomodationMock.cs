using Database.Models;

namespace TestFoxBe.Mocks;

public partial class DbMock
{
    public static List<Accomodation> Accomodations = new()
    {
        new Accomodation(){ Id = 1, Address = "Via Test 1", Name = "Hotel 1" },
        new Accomodation(){ Id = 2, Address = "Via Test 2", Name = "Hotel 2" },
        new Accomodation(){ Id = 3, Address = "Via Test 3", Name = "Hotel 3" },
    };
}