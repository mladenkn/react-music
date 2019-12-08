import { backForwardDateNavigatorPreMap } from "./BackForwardDateNavigator";
import { getDate, parse } from "date-fns";
import { renderHook, act } from "react-hooks-testing-library";

describe('useBackForwardDateNavigatorState', () => {

    test('1', () => {

        const initialDate = parse('2019-01-01T11:30:30')
        const changes: Array<Date> = []
        const stateContext = renderHook(() => backForwardDateNavigatorPreMap({
            value: initialDate, 
            onChange: d => changes.push(d),
            lowerBound: initialDate
        }))

        const lastDateDay = () => getDate(changes[changes.length - 1])

        act(() => stateContext.result.current.onForward())
        expect(lastDateDay()).toBe(2)
    })
    
    test('disable back when on today', () => {
        const stateContext = renderHook(() => backForwardDateNavigatorPreMap({
            value: new Date(),
            lowerBound: new Date(),
            onChange: () => {},
        }))          
        expect(stateContext.result.current.backDisabled).toBe(true)
    })
})

