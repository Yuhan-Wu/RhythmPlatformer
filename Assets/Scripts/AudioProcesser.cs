using System.Collections;
using System.Collections.Generic;
using System;
using System.Numerics;
using DSPLib;
using UnityEngine;

public class AudioProcesser
{

    private float ClipLength = 0;
    private float SampleRate = 0;
    private int NumOfTotalSamples = 0;
    private AudioClip Clip;
    private SpectralFluxAnalyzer Analyzer;

    public AudioProcesser(AudioClip p_Clip)
    {
        Clip = p_Clip;
        Analyzer = new SpectralFluxAnalyzer();
    }

    public List<SpectralFluxInfo> Process()
    {
        try
        {
            float[] samples = new float[Clip.samples * Clip.channels];
            int numOfChannels = Clip.channels;
            this.NumOfTotalSamples = Clip.samples;
            this.ClipLength = Clip.length;
            this.SampleRate = Clip.frequency;
            Clip.GetData(samples, 0);

            // Combine stereo type
            float[] preProcessedSamples = new float[this.NumOfTotalSamples];
            int numProcessed = 0;
            float combinedChannelAverage = 0;
            for (var i = 0; i < samples.Length; i++)
            {
                combinedChannelAverage += samples[i];
                if ((i + 1) % numOfChannels == 0)
                {
                    preProcessedSamples[numProcessed] = combinedChannelAverage / numOfChannels;
                    numProcessed++;
                    combinedChannelAverage = 0;
                }
            }

            // Execute an FFT to return the spectrum data over the time domain
            int spectrumSampleSize = 1024;
            int iterations = preProcessedSamples.Length / spectrumSampleSize;

            FFT fft = new FFT();
            fft.Initialize((UInt32)spectrumSampleSize);

            double[] sampleChunk = new double[spectrumSampleSize];
            for (int i = 0; i < iterations; i++)
            {
                // Grab the current 1024 chunk of audio sample data
                Array.Copy(preProcessedSamples, i * spectrumSampleSize, sampleChunk, 0, spectrumSampleSize);

                // Apply our chosen FFT Window
                double[] windowCoefs = DSP.Window.Coefficients(DSP.Window.Type.Hanning, (uint)spectrumSampleSize);
                double[] scaledSpectrumChunk = DSP.Math.Multiply(sampleChunk, windowCoefs);
                double scaleFactor = DSP.Window.ScaleFactor.Signal(windowCoefs);

                // Perform the FFT and convert output (complex numbers) to Magnitude
                Complex[] fftSpectrum = fft.Execute(scaledSpectrumChunk);
                double[] scaledFFTSpectrum = DSPLib.DSP.ConvertComplex.ToMagnitude(fftSpectrum);
                scaledFFTSpectrum = DSP.Math.Multiply(scaledFFTSpectrum, scaleFactor);

                // These 1024 magnitude values correspond (roughly) to a single point in the audio timeline
                float curSongTime = getTimeFromIndex(i) * spectrumSampleSize;

                // Send our magnitude data off to our Spectral Flux Analyzer to be analyzed for peaks
                Analyzer.analyzeSpectrum(Array.ConvertAll(scaledFFTSpectrum, x => (float)x), curSongTime);
            }

            return Analyzer.spectralFluxSamples;

        }
        catch (Exception e)
        {
            // Catch exceptions here since the background thread won't always surface the exception to the main thread
            Debug.Log(e.ToString());
        }
        return null;

    }

    private float getTimeFromIndex(int index)
    {
        return ((1f / (float)this.SampleRate) * index);
    }

}
