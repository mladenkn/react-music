import { useImmer } from "use-immer"

export const useAdminSectionLogic = () => {

  const queries = [
    {
      name: 'Query 1',
      yaml: `name: Martin D'vloper
job: Developer
skill: Elite
employed: True
foods:
  - Apple
  - Orange
  - Strawberry
  - Mango
languages:
  perl: Elite
  python: Elite
  pascal: Lame
education: |
  4 GCSEs
  3 A-Levels
  BSc in the Internet of Things 
`
    },
    {
      name: 'Query 2',
      yaml: `# A list of tasty fruits
- Apple
- Orange
- Strawberry
- Mango
`
    },
    {
      name: 'Query 3',
      yaml: `# Employee records
-  martin:
    name: Martin D'vloper
    job: Developer
    skills:
      - python
      - perl
      - pascal
-  tabitha:
    name: Tabitha Bitumen
    job: Developer
    skills:
      - lisp
      - fortran
      - erlang
`
    }
  ]

  const [state, updateState] = useImmer({
    activeQueryName: queries[0].name
  })

  const setActiveQueryName = (name: string) => {
    updateState(draft => {
      draft.activeQueryName = name
    })
  }

  const activeQuery = queries.find(q => q.name === state.activeQueryName)!

  const responseYaml = `
doe: "a deer, a female deer"
ray: "a drop of golden sun"
pi: 3.14159
xmas: true
french-hens: 3
calling-birds: 
  - huey
  - dewey
  - louie
  - fred
xmas-fifth-day: 
  calling-birds: four
  french-hens: 3
  golden-rings: 5
  partridges: 
    count: 1
    location: "a pear tree"
  turtle-doves: two
  `

  return { 
    queries: queries.map(q => q.name),
    activeQuery,
    setActiveQueryName,
    responseYaml
  }
}