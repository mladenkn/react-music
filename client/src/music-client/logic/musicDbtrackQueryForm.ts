import { MusicDbTrackQueryParams } from "../shared/trackQueryForm";
import { useFormik } from "formik";
import { useEffect, useState } from "react";
import { difference } from 'lodash'

type Field = keyof MusicDbTrackQueryParams

interface MusicDbTrackQueryFormLogic {
	values: MusicDbTrackQueryParams
	inactiveFields: Field[]
	availableTags: string[]
	isFieldActive(fieldName: Field): boolean
	setFieldActive(fieldName: Field): void
	setFieldInactive(fieldName: Field): void
	onFieldChange(fieldName: Field): (value: any) => void
}

interface MusicDbTrackQueryFormLogicProps {
	values: MusicDbTrackQueryParams
	onChange: (values: MusicDbTrackQueryParams) => void
}

const allFields: Field[] = ['titleContains', 'youtubeChannelId', 'mustHaveEveryTag', 'mustHaveAnyTag', 'yearRange']

const getInitialActiveFields = (params: MusicDbTrackQueryParams) => {
	const result: Field[] = []

	if(params.mustHaveAnyTag && params.mustHaveAnyTag.length > 0)
		result.push('mustHaveAnyTag')	
	if(params.mustHaveEveryTag && params.mustHaveEveryTag.length > 0)
		result.push('mustHaveEveryTag')	
	if(params.titleContains && params.titleContains !== '')
		result.push('titleContains')
	if(params.yearRange && Object.entries(params.yearRange).length > 0)
		result.push('yearRange')
	if(params.youtubeChannelId)
		result.push('youtubeChannelId')
	
	return result;
}

const getFieldDefaultValue = (field: Field) => {
	switch (field) {
    case "youtubeChannelId":
      return undefined;
    case "mustHaveEveryTag":
      return [];
    case "mustHaveAnyTag":
      return [];
    case "titleContains":
      return '';
    case "yearRange":
      return {};
  }
}

export const useMusicDbTrackQueryFormLogic = (props: MusicDbTrackQueryFormLogicProps): MusicDbTrackQueryFormLogic => {
	const form = useFormik({
		enableReinitialize: true,
		initialValues: props.values,
		onSubmit: () => { }
	})

	const [activeFields, setActiveFields] = useState<Field[]>(getInitialActiveFields(props.values))

	useEffect(() => {
		props.onChange(form.values)
	}, [form.values])

	const isFieldActive = (fieldName: Field) => {
		return activeFields.includes(fieldName)
	}

	const onFieldChange = (fieldName: Field) => {
		return (value: any) => {
			form.setFieldValue(fieldName, value)
		}
	}

	const setFieldInactive = (fieldName: Field) => {
		onFieldChange(fieldName)(getFieldDefaultValue(fieldName))
		setActiveFields(activeFields.filter(f => f !== fieldName))
	}

	const setFieldActive = (fieldName: Field) => {
		setActiveFields([...activeFields, fieldName])
	}

	const inactiveFields = difference(allFields, activeFields)
	
  const availableTags = ['trance', 'techno', 'house', 'acid']

	return { values: form.values, availableTags, inactiveFields, isFieldActive, onFieldChange, setFieldInactive, setFieldActive }
}