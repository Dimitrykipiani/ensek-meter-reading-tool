import React from 'react';
import CsvUploader from './components/CsvUploader';
import './App.css';



const App: React.FC = () => {
  return (
    <div className="App">
      <h1>ENSEK Meter Reading Tool</h1>
      <CsvUploader />
    </div>
  );
};

export default App;
