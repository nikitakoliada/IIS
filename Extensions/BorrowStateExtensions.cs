using IIS.Enums;
using MySql.Data.MySqlClient.Interceptors;

namespace IIS.Extensions;

public static class BorrowStateExtensions
{
    public static IList<BorrowState> NextPossibleStates(this BorrowState state)
    {
        if (state == BorrowState.Pending)
        {
            return [BorrowState.Accepted, BorrowState.Rejected];
        }

        if (state == BorrowState.Accepted)
        {
            return [BorrowState.Given];
        }

        if (state == BorrowState.Given)
        {
            return [BorrowState.Returned];
        }

        return [];
    }
}