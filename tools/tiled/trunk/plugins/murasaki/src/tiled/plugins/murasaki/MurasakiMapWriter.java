package tiled.plugins.murasaki;

import java.io.File;
import java.io.OutputStream;

import tiled.core.Map;
import tiled.core.TileSet;
import tiled.io.MapWriter;
import tiled.io.PluginLogger;

public class MurasakiMapWriter implements MapWriter {

	@Override
	public void writeMap(Map map, String filename) throws Exception {
		// TODO Auto-generated method stub

	}

	@Override
	public void writeMap(Map map, OutputStream out) throws Exception {
		// TODO Auto-generated method stub

	}

	@Override
	public void writeTileset(TileSet set, String filename) throws Exception {
		// TODO Auto-generated method stub

	}

	@Override
	public void writeTileset(TileSet set, OutputStream out) throws Exception {
		// TODO Auto-generated method stub

	}

	@Override
	public String getDescription() {
		return "Exporter for Murasaki Engine";
	}

	@Override
	public String getFilter() throws Exception {
		return "*.murasaki";
	}

	@Override
	public String getName() {
		return "Murasaki exporter";
	}

	@Override
	public String getPluginPackage() {
		return "Murasaki export plugin";
	}

	@Override
	public void setLogger(PluginLogger logger) {
		// TODO Auto-generated method stub

	}

	@Override
	public boolean accept(File pathname) {
		// TODO Auto-generated method stub
		return false;
	}

}
