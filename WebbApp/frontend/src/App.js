import './App.css';
import Routing from "./pages/Routing";
import { Helmet } from "react-helmet"
import NavBarPartial from './partials/navbar/NavBarPartial';
import { UserProvider } from './contexts/UserContext';

function App() {
  return (
   <>
    <Helmet>
      <script src="/asset/js/jsencrypt.min.js" async></script>
    </Helmet>
    <UserProvider>
      <NavBarPartial/>
      <Routing/>
    </UserProvider>
   </>
  );
}

export default App;
