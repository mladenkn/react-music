import { MusicDbTrackQueryParams } from "../shared/homeSectionOptions";
import { useFormik } from "formik";
import { useState } from "react";
import { difference } from 'lodash'
import { IdWithName } from "../../utils/types";
import { useEffect } from "../../utils/useEffect";

type Field = keyof MusicDbTrackQueryParams

interface MusicDbTrackQueryFormLogic {
	values: MusicDbTrackQueryParams
	includableFields: Field[]
	availableTags: string[]
	isFieldActive(fieldName: Field): boolean
	setFieldActive(fieldName: Field): void
	setFieldInactive(fieldName: Field): void
	onFieldChange(fieldName: Field): (value: any) => void
	removeRelatedTrack(trackId: number): void
  availableYouTubeChannels: IdWithName[]
}

interface MusicDbTrackQueryFormLogicProps {
	values: MusicDbTrackQueryParams
	onChange: (values: MusicDbTrackQueryParams) => void
  availableTags: string[]
  availableYouTubeChannels: IdWithName[]
}

const allFields: Field[] = ['titleContains', 'supportedYouTubeChannelsIds', 'mustHaveEveryTag', 'mustHaveAnyTag', 'musntHaveEveryTag', 'yearRange', 'randomize', 'relatedTracks']

const getInitialActiveFields = (params: MusicDbTrackQueryParams) => {
	const result: Field[] = ['randomize']

	if(params.mustHaveAnyTag?.length)
		result.push('mustHaveAnyTag')	
	if(params.mustHaveEveryTag?.length)
		result.push('mustHaveEveryTag')	
	if(params.musntHaveEveryTag?.length)
		result.push('musntHaveEveryTag')	
	if(params.titleContains && params.titleContains !== '')
		result.push('titleContains')
	if(params.yearRange?.lowerBound || params.yearRange?.upperBound)
		result.push('yearRange')
	if(params.supportedYouTubeChannelsIds?.length)
		result.push('supportedYouTubeChannelsIds')
	if(params.relatedTracks?.length)
		result.push('relatedTracks')
		
	return result;
}

const getFieldDefaultValue = (field: Field) => {
	switch (field) {
    case "supportedYouTubeChannelsIds":
      return [];
    case "mustHaveEveryTag":
      return [];
    case "mustHaveAnyTag":
      return [];
		case "musntHaveEveryTag":
			return [];
    case "titleContains":
      return '';
    case "randomize":
      return true;
		case "yearRange":
			return {};
		case "relatedTracks":
			return [];
  }
}

export const useMusicDbTrackQueryFormLogic = (props: MusicDbTrackQueryFormLogicProps): MusicDbTrackQueryFormLogic => {
	const form = useFormik({
		enableReinitialize: true,
		initialValues: props.values,
		onSubmit: () => { }
	})

	const [activeFields, setActiveFields] = useState<Field[]>(getInitialActiveFields(props.values))

	console.log(activeFields)

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

	const includableFields = difference(allFields, activeFields).filter(f => f !== 'relatedTracks')

	const removeRelatedTrack = (trackId: number) => {
		const newRelatedTracks = form.values.relatedTracks.filter(t => t !== trackId)
		form.setFieldValue('relatedTracks', newRelatedTracks);
	}

	return { 
    values: form.values, 
    availableYouTubeChannels: props.availableYouTubeChannels, 
    availableTags: props.availableTags, 
    includableFields: includableFields, 
    isFieldActive, 
    onFieldChange, 
    setFieldInactive, 
		setFieldActive,
		removeRelatedTrack
  }
}