import { parse } from "date-fns";
import { getHoursDecimal } from ".";

test('getHourOfDayDecimal', () => {
    const initialDate = parse('2019-01-01T11:30:30')
    expect(getHoursDecimal(initialDate)).toBe(11.5)
})