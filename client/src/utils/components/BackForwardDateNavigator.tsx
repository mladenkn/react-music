import { getDate, addDays } from 'date-fns';
 
interface BackForwardDateNavigatorProps {
    value: Date
    lowerBound?: Date
    onChange: (value: Date) => void
}

export const backForwardDateNavigatorPreMap = (p: BackForwardDateNavigatorProps) => {
    const backDisabled = p.lowerBound ? (getDate(p.value) - getDate(p.lowerBound)) === 0 : false
    return {
        onBack: () => p.onChange(addDays(p.value, -1)),
        onForward: () => p.onChange(addDays(p.value, 1)),
        backDisabled
    }
}