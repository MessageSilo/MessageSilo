"use client"

import { useEffect, createContext, useContext, useState } from 'react';
import * as signalR from "@microsoft/signalr";
import useSWR from 'swr'
import axios from 'axios'
import useDashboardStore from '../store/dashboard';

const fetcher = (url) =>
  axios.get(url, { headers: { authorization: '00000000-0000-0000-0000-000000000000' } })
    .then(res => res.data);

export default function Home() {
  const entities = useDashboardStore((state) => state.entities);
  const events = useDashboardStore((state) => state.events);
  const populateEntitites = useDashboardStore((state) => state.populateEntitites);
  const addEvent = useDashboardStore((state) => state.addEvent);
  const { data, error } = useSWR(`${process.env.NEXT_PUBLIC_API_URL}/entities`, fetcher)

  useEffect(() => {
    populateEntitites(data);

    const connection = new signalR.HubConnectionBuilder()
      .withUrl(`${process.env.NEXT_PUBLIC_API_BASE_URL}/signal`)
      .configureLogging(signalR.LogLevel.Information)
      .build();

    async function start() {
      try {
        await connection.start();
        console.log("SignalR Connected");
        await connection.send("connect");
      } catch (err) {
        console.log(err);
        setTimeout(start, 5000);
      }
    };

    connection.onclose(async () => {
      console.log("closed!!!!!");
      await start();
    });

    connection.on("connected", () => {
      console.log("Client Connected");
    });

    connection.on("signalReceived", (signal) => {
      addEvent(signal);
    });

    start();
  })

  return (
    <div>
      <h1 className="text-3xl text-black pb-6">Dashboard</h1>
      <table>
        <thead>
          <tr>
            <th>Name</th>
            <th>Kind</th>
            <th>Events</th>
          </tr>
        </thead>
        <tbody>
          {entities?.map(p => (
            <tr key={p.id}>
              <td>{p.name}</td>
              <td>{p.kind}</td>
              <td>{JSON.stringify(events?.find(x => x.entityId.startsWith(`${p.name}#`)))}</td>
            </tr>))}
        </tbody>
      </table>
    </div>
  )
}
