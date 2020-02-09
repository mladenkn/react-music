import { MusicDbTrackQueryForm } from "../shared";

type Field = keyof MusicDbTrackQueryForm

interface MusicDbTrackQueryFormLogic {
    isFieldActive(fieldName: Field): boolean
    setFieldActive(fieldName: Field): void
    setFieldInactive(fieldName: Field): void
    values: MusicDbTrackQueryForm
    onFieldChange(fieldName: Field): (value: any) => void
    getFieldProps(fieldName: Field): object
}

export const useMusicDbTrackQueryFormLogic = (form: MusicDbTrackQueryForm) => {
    const values = form
    
    const isFieldActive = (fieldName: Field) => {
        return true
    }

    const onFieldChange = (fieldName: Field) => {
        return (value: any) => {

        }
    }

    const setFieldInactive = (fieldName: Field) => {
        
    }

    return { values, isFieldActive, onFieldChange, setFieldInactive }
}