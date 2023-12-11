"use client"

import { useEffect } from 'react';
import * as signalR from "@microsoft/signalr";
import useSWR from 'swr'
import axios from 'axios'

const fetcher = (url) =>
  axios.get(url, { headers: { authorization: '00000000-0000-0000-0000-000000000000' } })
    .then(res => res.data);

export default function Home() {
  const { data, error } = useSWR(`${process.env.NEXT_PUBLIC_API_URL}/entities`, fetcher)

  useEffect(() => {
    console.log(data, error);

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
      console.log(signal);
    });

    start();
  })

  return (
    <div>
      <h1 className="text-3xl text-black pb-6">Dashboard</h1>
      TEST
    </div>
  )
}
