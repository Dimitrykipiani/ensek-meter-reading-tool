import React, { useState } from 'react';
import type { ChangeEvent, FormEvent } from 'react';
import './CsvUploader.css'; // 👈 Create this next

type UploadResult = {
  successfulReadings: number;
  failedReadings: number;
  errors: string[];
};


const CsvUploader: React.FC = () => {
  const [file, setFile] = useState<File | null>(null);
  const [result, setResult] = useState<UploadResult | null>(null);
  const [error, setError] = useState<string | null>(null);

  const handleFileChange = (e: ChangeEvent<HTMLInputElement>) => {
    setFile(e.target.files?.[0] || null);
    setResult(null);
    setError(null);
  };

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();
    if (!file) {
      setError("Please select a CSV file.");
      return;
    }

    const formData = new FormData();
    formData.append('file', file);

    try {
      const response = await fetch('https://localhost:7211/MeterReadingUpload/meter-reading-uploads', {
        method: 'POST',
        body: formData
      });

      if (!response.ok) {
        const errorBody = await response.json();
        throw new Error(errorBody?.detail || 'Upload failed');
      }

      const data: UploadResult = await response.json();
      setResult(data);
    } catch (err: any) {
      setError(err.message || 'Something went wrong');
    }
  };

  return (
    <div className="upload-container">
      <h2>📄 Meter Reading Uploader</h2>

      <form onSubmit={handleSubmit} className="upload-form">
        <input type="file" accept=".csv" onChange={handleFileChange} />
        <button type="submit">Upload</button>
      </form>

      {error && (
        <div className="error-box">
          <p>❌ {error}</p>
        </div>
      )}

      {result && (
        <div className="result-box">
          <h3>✅ Upload Summary</h3>
          <p>✔️ Successful Readings: <strong>{result.successfulReadings}</strong></p>
          <p>❌ Failed Readings: <strong>{result.failedReadings}</strong></p>

          {result.errors.length > 0 && (
            <div className="error-list">
              <h4>Details:</h4>
              <ul>
                {result.errors.map((err, idx) => (
                  <li key={idx}>– {err}</li>
                ))}
              </ul>
            </div>
          )}
        </div>
      )}
    </div>
  );
};

export default CsvUploader;
