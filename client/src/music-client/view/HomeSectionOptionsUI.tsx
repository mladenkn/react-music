import React, { useEffect } from 'react'
import { TracklistOptions, HomeSectionOptions } from "../shared/homeSectionOptions"
import { InputLabel, Select, MenuItem, Switch, TextField, makeStyles, createStyles, Button, Checkbox } from "@material-ui/core"
import { MusicDbTrackQueryInteractiveForm } from './MusicDbTrackQueryInteractiveForm'
import { useFormik } from 'formik'
import clsx from 'clsx'
import { ems, percent } from '../../utils/css'
import { snapshot } from '../../utils'

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
			marginTop: ems(0.5),
			maxWidth: percent(83),
		},
		input: {
			display: 'flex',
			alignItems: 'center',
    },
    autoRefreshInput: {
      marginTop: ems(1),
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
	
	console.log(form.values.tracklist.queryForm.youTubeQuery!, snapshot(form.values), snapshot(props.values))

	return (
		<div className={clsx(props.className, styles.root)}>
			<Select
				className={styles.dataSource}
				label='Data source'
				value={form.values.tracklist.queryForm.dataSource}
				onChange={e => form.setFieldValue('tracklist.queryForm.dataSource', e.target.value)}
			>
				<MenuItem value='MusicDb'>Music DB</MenuItem>
				<MenuItem value='YouTube'>YouTube</MenuItem>
			</Select>

			{form.values.tracklist.queryForm.dataSource === 'MusicDb' &&
				<MusicDbTrackQueryInteractiveForm
					className={styles.fields}
					input={form.values.tracklist.queryForm.musicDbQuery!}
					onChange={value => form.setFieldValue('tracklist.queryForm.musicDbQuery', value)}
				/>
			} 

			{form.values.tracklist.queryForm.dataSource === 'YouTube' &&
				<TextField
					className={styles.searchQueryField}
					label='Search Query'
					value={form.values.tracklist.queryForm.youTubeQuery!}
					onChange={e => form.setFieldValue('tracklist.queryForm.youTubeQuery', e.target.value)}
				/>
			}
			
			<div className={clsx(styles.input, styles.autoRefreshInput)}>
				<InputLabel>Auto refresh:</InputLabel>
				<Switch 
					checked={form.values.tracklist.autoRefresh} 
					onChange={e => form.setFieldValue('tracklist.autoRefresh', e.target.checked)}
					color='primary'
				/>
			</div>
			
			<div className={styles.input}>
				<InputLabel>Auto play:</InputLabel>
				<Switch 
					checked={form.values.tracklist.autoPlay} 
					onChange={e => form.setFieldValue('tracklist.autoPlay', e.target.checked)}
					color='primary'
				/>
			</div>
			
			<div className={styles.input}>
				<InputLabel>Tracklist shown:</InputLabel>
				<Checkbox
					checked={form.values.tracklistShown}
					color='primary'
					onChange={e => form.setFieldValue('tracklistShown', e.target.checked)}
				/>
			</div>

			<Button className={styles.searchButton} color="primary" onClick={props.onSearch}>Search</Button>
		</div>
	)
}