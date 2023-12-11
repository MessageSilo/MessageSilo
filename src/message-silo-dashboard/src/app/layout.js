import './globals.css'

export const metadata = {
  title: 'Message Silo Dashboard',
  description: 'A tool to fix or enrich messages with the power of AI, and make integration simpler for event-driven systems. Azure Service Bus, AWS SQS, RabbitMQ, and more...',
}

export default function RootLayout({ children }) {
  return (
      <html lang="en">
        <body className="bg-gray-100 font-family-karla flex">
          <aside className="relative bg-sidebar h-screen w-64 hidden sm:block shadow-xl">
            <div className="p-6">
              <a href="index.html" className="text-white text-3xl font-semibold uppercase hover:text-gray-300">Message Silo</a>
              <button className="w-full bg-white cta-btn font-semibold py-2 mt-5 rounded-br-lg rounded-bl-lg rounded-tr-lg shadow-lg hover:shadow-xl hover:bg-gray-300 flex items-center justify-center">
                <i className="fas fa-plus mr-3"></i> New Flow
              </button>
            </div>
            <nav className="text-white text-base font-semibold pt-3">
              <a href="index.html" className="flex items-center text-white opacity-75 hover:opacity-100 py-4 pl-6 nav-item">
                <i className="fas fa-tachometer-alt mr-3"></i>
                Dashboard
              </a>
            </nav>
            <a href="#" className="absolute w-full upgrade-btn bottom-0 active-nav-link text-white flex items-center justify-center py-4">
              <i className="fas fa-arrow-circle-up mr-3"></i>
              Sponsor
            </a>
          </aside>

          <div className="relative w-full flex flex-col h-screen overflow-y-hidden">

            <header className="w-full items-center bg-white py-2 px-6 hidden sm:flex">
              <div className="w-1/2 font-bold">Message Silo</div>
              <div className="relative w-1/2 flex justify-end">
                <button className="realtive z-10 w-12 h-12 rounded-full overflow-hidden border-4 border-gray-400 hover:border-gray-300 focus:border-gray-300 focus:outline-none">
                  <img src="https://cdn.icon-icons.com/icons2/1371/PNG/512/robot02_90810.png" />
                </button>
              </div>
            </header>


            <header className="w-full bg-sidebar py-5 px-6 sm:hidden">
              <div className="flex items-center justify-between">
                <a href="index.html" className="text-white text-3xl font-semibold uppercase hover:text-gray-300">Admin</a>
                <button className="text-white text-3xl focus:outline-none">
                  <i className="fas fa-bars"></i>
                  <i className="fas fa-times"></i>
                </button>
              </div>


              <nav className="flex flex-col pt-4">
                <a href="index.html" className="flex items-center text-white opacity-75 hover:opacity-100 py-2 pl-4 nav-item">
                  <i className="fas fa-tachometer-alt mr-3"></i>
                  Dashboard
                </a>
                <button className="w-full bg-white cta-btn font-semibold py-2 mt-3 rounded-lg shadow-lg hover:shadow-xl hover:bg-gray-300 flex items-center justify-center">
                  <a href="https://github.com/sponsors/berkid89"><i className="fas fa-arrow-circle-up mr-3"></i>Sponsor</a>
                </button>
              </nav>
            </header>

            <div className="w-full h-screen overflow-x-hidden border-t flex flex-col">
              <main className="w-full flex-grow p-6">
                {children}
              </main>

              <footer className="w-full bg-white text-right p-4 sm:flex">
                <div className="w-10/12"></div>
                <div className="w-1/12"><a href="https://github.com/MessageSilo/MessageSilo/wiki">Documentation</a></div>
                <div className="w-1/12"><a href="https://github.com/MessageSilo/MessageSilo">GitHub</a></div>
              </footer>
            </div>

          </div>

        </body>
      </html>
  )
}
