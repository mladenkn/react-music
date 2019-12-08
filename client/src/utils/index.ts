import { Styles } from 'jss';
import { makeStyles, createStyles as createStyles_ } from '@material-ui/styles';
import { ComponentType, useState } from 'react';
import { Subtract, $ElementType as $ElementType_ } from 'utility-types';
import React, { useEffect } from 'react';
import { getMinutes, getHours, setHours, setMinutes, addDays } from 'date-fns'
import { Db, Collection } from 'mongodb'
import { assocPath } from 'ramda';

export const generateArray = <T> (getNext: () => T, count: number) => {;
    const r: T[] = [];
    for (let i = 0; i < count; i++) 
        r.push(getNext());
    return r;
}

export type AsyncOperationStatus = 'NEVER_INITIATED' | 'PROCESSING' | 'COMPLETED';

export const replaceMatches = <T> (arr: T[], doesMatch: (item: T) => boolean, replaceWith: T) => {
    const {allItems, updatedItems} = updateMatches(arr, doesMatch, () => replaceWith);
    return {allItems, newItems: updatedItems};
}

export const updateMatches = <T> (arr: T[], doesMatch: (item: T) => boolean, update: (item: T) => T) => {
    const allItems: T[] = [];
    const updatedItems: T[] = [];

    arr.forEach((item) => {
        if(doesMatch(item)){
            const updated = update(item);
            allItems.push(updated);
            updatedItems.push(updated);
        }
        else
            allItems.push(item);
    });

    return {allItems, updatedItems};
}

export const validURL = (str: string) => {
    var pattern = new RegExp(/^(?:\w+:)?\/\/([^\s\.]+\.\S{2}|localhost[\:?\d]*)\S*$/);
    return !!pattern.test(str);
}

export const containsOnlyDigits = (str: string) => {
    for (let index = 0; index < str.length; index++) {
        const c = str[index];
        if(isNaN(parseInt(c)))
            return false;
    }
    return true;
}

export const capitalize = (str: string) => {
    const firstUpper = str[0].toUpperCase();
    const withoutFirst = str.slice(1, str.length - 1);
    return firstUpper + withoutFirst;
}

export const createStyles = <C extends string> (styles: Styles<C>, name?: string) => 
    makeStyles(createStyles_(styles), {name});

export const connectToState = <TState, TStateParams, TComponentPartialProps extends {}, TComponentFullProps extends TComponentPartialProps>(
    a: {component: ComponentType<TComponentFullProps>, to: (p: TStateParams) => TState, with: (s: TState) => TComponentPartialProps}) => {
        return (restOfProps: Subtract<TComponentFullProps, TComponentPartialProps> & {state: TState}) => {
            const injectedProps = a.with(restOfProps.state)
            return React.createElement(a.component, ({...injectedProps, ...restOfProps} as unknown) as TComponentFullProps)
        }
} 

export const addPreMap = <TPreMapParams, TComponentPartialProps extends {}, TComponentFullProps extends TComponentPartialProps>(
    preMap: (p: TPreMapParams) => TComponentPartialProps, component: ComponentType<TComponentFullProps>) => {
        return (props: Subtract<TComponentFullProps, TComponentPartialProps> & TPreMapParams) => {
            const injectedProps = preMap(props)
            return React.createElement(component, ({...props, ...injectedProps} as unknown) as TComponentFullProps)
        }
}

export const withSth = <TPreMapParams, TComponentPartialProps extends {}, TComponentFullProps extends TComponentPartialProps>(
    preMap: (p: TPreMapParams) => TComponentPartialProps, component: ComponentType<TComponentFullProps>) => {
        return (props: Subtract<TComponentFullProps, TComponentPartialProps> & TPreMapParams) => {
            const injectedProps = preMap(props)
            return React.createElement(component, ({...props, ...injectedProps} as unknown) as TComponentFullProps)
        }
}

export const getHoursDecimal = (dateTime: Date) => getHours(dateTime) + getMinutesDecimal(dateTime)

export const getMinutesDecimal = (dateTime: Date) => getMinutes(dateTime) / 60

export const setTimeOfDay = (dateTime: Date, hours: number, minutes: number) => 
    setMinutes(setHours(dateTime, hours), minutes)

export const dateFromNow = (a: {days: number}) => addDays(new Date(), a.days)

export type $ElementType<TArray extends Iterable<any>> = $ElementType_<TArray, number>

export abstract class MongoBaseRepository<T> {

    protected readonly collection: Collection;

    constructor(db: Db, collectionName: string){
        this.collection = db.collection(collectionName)
    }
    
    async insert(item: T) {
        return this.collection.insertOne(item)
    }
    
    async insertMany(items: T[]) {
        return this.collection.insertMany(items)
    }
}

export const useFormLogicWithState = <TInput> (dataInitial: TInput, onChange?: (a: TInput) => void): FormLogic<TInput> => {
    const [inputCurrent, setInputCurrent] = useState(dataInitial)    
    useEffect(() => {
        onChange && onChange(inputCurrent) 
    }, [inputCurrent])
    return useFormLogic(inputCurrent, setInputCurrent)
}

export const useFormLogic = <TInput> (
    input: TInput, 
    onChange?: (a: TInput) => void, 
    mapChange?: (i: TInput) => TInput, 
    mapInput?: (i: TInput) => TInput
) : FormLogic<TInput> => {

    const onChange_ = onChange || ((i: TInput) => {})
    const mapChange_ = mapChange || ((i: TInput) => i)
    const mapInput_ = mapInput || ((i: TInput) => i)

    const setProp = (key: keyof TInput | [keyof TInput, string], value: unknown): TInput => {
        if(typeof key === 'string')
            return ({ ...input, [key]: value })
        else if (Array.isArray(key))
            return assocPath(key as string[], value, input) as TInput
        else
            throw new Error()
    }

    const getValue = (valueOrEvent: any) => {
        if(!valueOrEvent)
            return valueOrEvent
        if(valueOrEvent.target){
            const {type, value} = valueOrEvent.target
            if(type === 'number'){
                if (typeof value === 'number')
                    return value
                else if (typeof value === 'string')
                    return parseInt(value)
                else
                    throw new Error()
            }
            if(type === 'text')
                return value
        }
        else 
            return valueOrEvent
    }

    const onPropChange = (key: keyof TInput | [keyof TInput, string]) => 
        (valueOrEvent: unknown | {target: {value: unknown}}) => 
        {
            const newInput = setProp(key, getValue(valueOrEvent))
            onChange_(mapChange_(newInput))
        }
    
    return { input: mapInput_(input), onPropChange }
}
 
export interface FormLogic<TInput> {
    input: TInput
    onPropChange: (key: keyof TInput | [keyof TInput, string]) => (value: unknown | {target: {value: unknown}}) => void
} 