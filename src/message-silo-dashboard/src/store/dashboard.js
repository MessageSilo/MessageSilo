import { create } from 'zustand'

const useDashboardStore = create((set) => ({
  entities: [],
  events: [],
  populateEntitites: (listOfEntitites) => set((state) => ({ entities: listOfEntitites})),
  addEvent: (event) => set((state) => ({ events: [event, ...state.events]})),
}))

export default useDashboardStore;