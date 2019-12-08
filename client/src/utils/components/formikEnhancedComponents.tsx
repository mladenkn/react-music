import React, { ReactNode } from 'react'
import { ChangeEvent, ComponentType } from "react";
import { FormikConsumer, getIn } from "formik";
import { TextField as  MuiTextField, Select as MuiSelect } from '@material-ui/core'
import { TextFieldProps } from "@material-ui/core/TextField";

interface FieldProps {
    name?: string
    onChange?: (e: ChangeEvent<any>, child?: ReactNode) => void
}

export function echnaceWithFormik<TProps extends FieldProps>(Component: ComponentType<FieldProps>){
    return (props: TProps) => (
        <FormikConsumer>
            {c => {
                const value = props.name ? getIn(c.values, props.name!) : ''
                return <Component {...props} onChange={c.handleChange} value={value} />
            } }
        </FormikConsumer>
    )
}

export const TextField = echnaceWithFormik<TextFieldProps>(MuiTextField)
export const Select = echnaceWithFormik<TextFieldProps>(MuiSelect)