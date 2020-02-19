import React, { useEffect } from 'react'
import { HomeSectionOptions } from "../shared/homeSectionOptions"
import { InputLabel, Select, MenuItem, Switch, TextField, makeStyles, createStyles, Button } from "@material-ui/core"
import { MusicDbTrackQueryInteractiveForm } from './MusicDbTrackQueryInteractiveForm'
import { useFormik } from 'formik'
import clsx from 'clsx'
import { ems, percent } from '../../utils/css'

interface HomeSectionOptionsUIProps {
	className?: string
	values: HomeSectionOptions
	onChange: (f: HomeSectionOptions) => void
	onSearch: () => void
}

const useStyles = makeStyles(
	() => createStyles({
		root: {
			display: 'flex',
			flexDirection: 'column',
			height: 'fit-content',
		},
		dataSource: {
		},
		fields: {
			marginTop: ems(1),
		},
		searchQueryField: {
			marginTop: ems(1),
			maxWidth: percent(83),
		},
		switchInput: {
			display: 'flex',
			alignItems: 'center',
		},
		searchButton: {			
			width: ems(5.5),
			alignSelf: 'flex-end',
		}
	}), {name: 'TrackQueryFormUi'}
)

export const HomeSectionOptionsUI = (props: HomeSectionOptionsUIProps) => {

	const styles = useStyles()
	const form = useFormik({
		enableReinitialize: true,
		initialValues: props.values,
		onSubmit: () => {}
	})
	useEffect(() => {
		props.onChange(form.values)
	}, [form.values])

	return (
		<div className={clsx(props.className, styles.root)}>
			<Select
				className={styles.dataSource}
				label='Data source'
				value={form.values.tracksQueryForm.dataSource}
				onChange={e => form.setFieldValue('tracksQueryForm.dataSource', e.target.value)}
			>
				<MenuItem value='MusicDb'>Music DB</MenuItem>
				<MenuItem value='YouTube'>YouTube</MenuItem>
			</Select>

			{form.values.tracksQueryForm.dataSource === 'MusicDb' &&
				<MusicDbTrackQueryInteractiveForm
					className={styles.fields}
					input={form.values.tracksQueryForm.musicDbParams!}
					onChange={value => form.setFieldValue('tracksQueryForm.musicDbParams', value)}
				/>
			} 

			{form.values.tracksQueryForm.dataSource === 'YouTube' &&
				<TextField
					className={styles.searchQueryField}
					label='Search Query'
					value={form.values.tracksQueryForm.searchQuery!}
					onChange={e => form.setFieldValue('tracksQueryForm.searchQuery', e.target.value)}
				/>
			}
			
			<div className={styles.switchInput}>
				<InputLabel>Auto refresh:</InputLabel>
				<Switch 
					checked={form.values.autoRefresh} 
					onChange={e => form.setFieldValue('autoRefresh', e.target.checked)}
					color='primary'
				/>
			</div>
			
			<div className={styles.switchInput}>
				<InputLabel>Auto play:</InputLabel>
				<Switch 
					checked={form.values.autoPlay} 
					onChange={e => form.setFieldValue('autoPlay', e.target.checked)}
					color='primary'
				/>
			</div>

			<Button className={styles.searchButton} color="primary" onClick={props.onSearch}>Search</Button>
		</div>
	)
}